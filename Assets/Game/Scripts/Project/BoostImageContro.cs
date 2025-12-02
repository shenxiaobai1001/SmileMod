using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoostImageContro : MonoBehaviour
{
    public Image targetImage;          // 目标Image组件
    public float reductionDuration = 50;    // 填充减少总时间
    public float recoveryDuration = 2f;    // 填充恢复总时间

    private float remainingTime = 0f;     // 剩余时间
    private Coroutine currentCoroutine;   // 当前运行的协程
    private bool isReducing = false;      // 是否正在减少状态

    // 对外暴露的剩余时间属性（秒）
    public float RemainingTime => remainingTime;
    public bool IsReducing => isReducing;

    private void Start()
    {
        if (targetImage) targetImage.gameObject.SetActive(false);
        remainingTime = reductionDuration; // 初始满时间
    }

    // 开始减少填充和时间
    public void StartReduction()
    {
        targetImage.gameObject.SetActive(true);
        isReducing = true;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ReductionRoutine());
    }

    // 在减少过程中重新开始减少（不打断协程）
    public void RestartReduction()
    {
        if (!isReducing) return;

        // 直接重置剩余时间，协程会继续运行但使用新的时间值
        remainingTime = reductionDuration;
        targetImage.fillAmount = 1f;

        Debug.Log($"重新开始减少，剩余时间: {remainingTime:F1}秒");
    }

    // 减少协程（修改为使用remainingTime变量）
    private IEnumerator ReductionRoutine()
    {
        while (isReducing && remainingTime > 0f)
        {
            // 每帧减少时间
            remainingTime -= Time.deltaTime;

            // 根据剩余时间计算填充量
            targetImage.fillAmount = remainingTime / reductionDuration;

            yield return null;
        }

        // 检查是否自然结束（remainingTime <= 0）
        if (remainingTime <= 0f)
        {
            OnReductionComplete();
        }
    }

    // 开始恢复填充和时间
    public void StartRecovery()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(RecoveryRoutine());
    }

    // 恢复协程
    private IEnumerator RecoveryRoutine()
    {
        float startFill = targetImage.fillAmount;
        float startTime = remainingTime;
        float elapsedTime = 0f;

        // 计算实际需要的持续时间（基于当前剩余比例）
        float actualDuration = recoveryDuration * (1f - startFill);

        while (elapsedTime < actualDuration)
        {
            float progress = elapsedTime / actualDuration;
            targetImage.fillAmount = Mathf.Lerp(startFill, 1f, progress);
            remainingTime = Mathf.Lerp(startTime, reductionDuration, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        OnRecoveryComplete();
    }

    // 减少完成回调
    protected virtual void OnReductionComplete()
    {
        isReducing = false;
        targetImage.gameObject.SetActive(false);
        Debug.Log("减少过程自然结束");
    }

    // 恢复完成回调
    protected virtual void OnRecoveryComplete()
    {
        if (targetImage) targetImage.gameObject.SetActive(false);
        Debug.Log("恢复过程结束");
    }

    // 停止所有效果
    public void StopAll()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        isReducing = false;
        targetImage.gameObject.SetActive(false);
    }
}