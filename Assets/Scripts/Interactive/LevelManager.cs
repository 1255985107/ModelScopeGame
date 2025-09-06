using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject[] platformsToCollapse;
    [SerializeField] private GameObject deathUI;       // ��������
    [SerializeField] private GameObject jumpTutorialUI; // ��Ծ�̳�UI
    [SerializeField] private GameObject walkTutorialUI; // ���߽̳�UI

    [Header("Audio Clips")]
    [SerializeField] private AudioClip bgmChantClip;        // �ƶ˳�����ʥӽ���������֣��̳̽׶Σ�
    [SerializeField] private AudioClip bgmCollapseClip;     // �������������������֣������׶Σ�
    [SerializeField] private AudioClip sfxDataBreakClip;    // ������������ƽ̨����ʱ���ŵ���Ч��
    [SerializeField] private AudioClip sfxArcClip;          // �绡��������׹��򿿽����ѵ���ʱ���ţ�
    [SerializeField] private AudioClip windClip;            // ������Ч������������������׹���߿ճ�����
    private AudioSource audioSource; // ���ڲ�����Ч

    [Header("Act Control")]
    private bool isGameOver = false;
    private bool hasLearnedJump = false; // �������Ƿ���ѧ����Ծ

    void Start()
    {
        SetupReferences();
        SetupEvents();
        InitializeLevel();
    }

    private void SetupReferences()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (deathUI != null)
            deathUI.SetActive(false);

        if (jumpTutorialUI != null)
            jumpTutorialUI.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void SetupEvents()
    {
        if (playerController != null)
        {
            playerController.onDeath.AddListener(HandlePlayerDeath);
            playerController.onSuccessfulJump.AddListener(HandleSuccessfulJump);
        }
    }

    private void InitializeLevel()
    {
        // ��ʼ�׶Σ�ֻ������
        PlayBGM(bgmChantClip);
        playerController.DisableJump();
        playerController.EnableMovement();
        hasLearnedJump = false; // ��ʼ��Ϊδѧ����Ծ
    }

    // ֻ�����߽׶δ���
    public void OnlyWalkTrigger()
    {
        PlayBGM(bgmCollapseClip);
        playerController.DisableJump();
        if (walkTutorialUI != null)
            walkTutorialUI.SetActive(false);
        Debug.Log("���̣�ֻ�����߽׶�");
    }

    // ��Ծ��ѧ�׶δ���
    public void JumpTutorialTrigger()
    {
        StopBGM();
        PlayBGM(bgmCollapseClip);
        playerController.EnableJump();
        hasLearnedJump = true; // �����ѧ����Ծ
        Debug.Log("�����ѧ����Ծ����Ծ����������");
        if (jumpTutorialUI != null)
            jumpTutorialUI.SetActive(true);
        Debug.Log("���̣���Ծ��ѧ�׶�");
    }



    // ����׹�䴥��
    public void FinalFallTrigger()
    {
        //PlaySFX(sfxArcClip);
        playerController.DisableJump();
        playerController.DisableMovement();
        Debug.Log("���̣�����׹��׶�");
    }

    private void HandlePlayerDeath()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (deathUI != null)
            deathUI.SetActive(true);

        StartCoroutine(ResetLevelAfterDelay(2f));
    }

    private void HandleSuccessfulJump()
    {
        if (isGameOver) return;

        //playerController.DisableMovement();
        //playerController.DisableJump();
    }

    IEnumerator ResetLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ��Ч���Ÿ�������
    private void PlayBGM(AudioClip clip)
    {
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void StopBGM()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void OnTriggerZoneActivated()
    {
         if (walkTutorialUI != null && walkTutorialUI.activeSelf)
         {
            walkTutorialUI.SetActive(false);
            OnlyWalkTrigger();
          }
        else if (jumpTutorialUI != null && jumpTutorialUI.activeSelf)
        {
            jumpTutorialUI.SetActive(false);
            JumpTutorialTrigger();
        }
        Debug.Log("Tutorial Zone Completed!");
    }

    // ��Update�����ʱ����̬������ԾȨ��
    void Update()
    {
        if (playerController != null)
        {
            if (hasLearnedJump)
                playerController.EnableJump();
            else
                playerController.DisableJump();
        }
    }
}
