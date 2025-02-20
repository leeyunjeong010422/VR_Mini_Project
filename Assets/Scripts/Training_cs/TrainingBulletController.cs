using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingBulletController : MonoBehaviour
{
    private TrainingBulletPool bulletPool;
    private Rigidbody bulletRigidbody;
    private Coroutine returnCoroutine;

    private void Start()
    {
        bulletPool = FindObjectOfType<TrainingBulletPool>();
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            ReturnToPool(); //적과 충돌하면 풀로 반환
        }

        if (collider.CompareTag("Monster"))
        {
            ReturnToPool(); //몬스터와 충돌하면 풀로 반환
        }
    }

    private void OnEnable()
    {
        ResetBullet(); //총알 초기화
    }

    private void OnDisable()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine); //이전 코루틴 중단
        }
    }

    private void ResetBullet()
    {
        if (bulletRigidbody != null)
        {
            bulletRigidbody.velocity = Vector3.zero; //속도 초기화
            bulletRigidbody.angularVelocity = Vector3.zero; //회전 속도 초기화
        }
    }

    public void ReturnToPool()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine); //활성화 상태에서 코루틴 중단
        }

        bulletPool.ReturnBullet(gameObject); //총알을 풀로 반환
    }

    public void StartReturnCoroutine(float delay)
    {
        returnCoroutine = StartCoroutine(ReturnBulletAfterDelay(delay));
    }

    private IEnumerator ReturnBulletAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(); //지연 후 총알 반환
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target")) //과녁에 맞았을 때
        {
            TargetScore targetScore = collision.gameObject.GetComponent<TargetScore>();
            if (targetScore != null)
            {
                int score = targetScore.CalculateScore(collision.contacts[0].point); //점수 계산
                ScoreManager.Instance.UpdateScore(score); //점수판 업데이트
            }

            ReturnToPool(); //총알 풀로 돌아가기
        }
    }
}
