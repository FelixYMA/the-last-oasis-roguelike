using System;
using Cysharp.Threading.Tasks;
using GamePlay;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private static readonly int LightAttack = Animator.StringToHash("LightAttack");
    private static readonly int HeavyAttack = Animator.StringToHash("HeavyAttack");
    private static readonly int Move1 = Animator.StringToHash("Move");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Die1 = Animator.StringToHash("Die");
    private static readonly int Hurt = Animator.StringToHash("Hurt");

    [Header("move speed")]
    public bool movable;
    public float moveSpeed;
    public float intervalL = 1f;
    public float intervalR = 2f;
    public bool isAttack, isRun;
    // private int comboStep;
    private AttackController m_Attack;
    private Rigidbody2D m_Rb;
    private BoxCollider2D m_Box;
    private Animator m_Animator;
    private SpriteRenderer m_Renderer;
    private Vector2 m_Movement;
    private bool m_IsDead;
    private float m_LeftAttackTimer, m_RightAttackTimer;
    private void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Box = GetComponent<BoxCollider2D>();
        m_Animator = GetComponent<Animator>();
        m_Renderer = GetComponent<SpriteRenderer>();
        m_Attack = GetComponentInChildren<AttackController>();
        m_IsDead = isAttack = isRun = false;
        m_LeftAttackTimer = m_RightAttackTimer = Time.time;
        movable = true;
    }

    void FixedUpdate()
    {
        if (m_IsDead) return;
        Move();
        Attack();
    }

    private void Move()
    {
        if (!movable || isAttack)
        {
            m_Animator.SetBool(Move1, false);
            m_Animator.SetBool(Run, false);
            m_Rb.linearVelocity = Vector2.zero;
            return;
        }
        m_Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isRun = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
        if(m_Movement.magnitude <= 0.01f)
        {
            m_Animator.SetBool(Move1, false);
            m_Animator.SetBool(Run, false);
        }
        else
        {
            m_Animator.SetBool(isRun ? Run : Move1, true);
            transform.localScale = m_Movement.x switch
            {
                > 0 => new Vector3(1, 1, 1),
                < 0 => new Vector3(-1, 1, 1),
                _ => transform.localScale
            };
        }
        m_Rb.linearVelocity = m_Movement * (isRun ? moveSpeed * 2.0f : moveSpeed);
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttack && Time.time - m_LeftAttackTimer > intervalL)
        {
            m_LeftAttackTimer = Time.time;
            isAttack = true;
            m_Animator.SetTrigger(LightAttack);
            m_Attack.gameObject.SetActive(true);
            m_Box.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && !isAttack && Time.time - m_RightAttackTimer > intervalR)
        {
            m_RightAttackTimer = Time.time;
            isAttack = true;
            m_Animator.SetTrigger(HeavyAttack);
            m_Attack.gameObject.SetActive(true);
            m_Box.enabled = false;
        }
    }

    public void AttackOver()
    {
        isAttack = false;
        m_Attack.gameObject.SetActive(false);
        m_Box.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.name)
        {
            case "Door_up":
                MoveInDoor(other, Vector3.up).Forget();
                break;
            case "Door_left":
                MoveInDoor(other, Vector3.left).Forget();
                break;
            case "Door_right":
                MoveInDoor(other, Vector3.right).Forget();
                break;
            case "Door_down":
                MoveInDoor(other, Vector3.down).Forget();
                break;
            case "Coin(Clone)":
                other.gameObject.SetActive(false);
                GameManager.Ist.uiAudio.PlayOneShot(GameManager.Ist.coinAudio);
                GameUI.Ist.AddScore().Forget();
                break;
            case "Food(Clone)":
                other.gameObject.SetActive(false);
                GameManager.Ist.uiAudio.PlayOneShot(GameManager.Ist.foodAudio);
                GameUI.Ist.AddHp().Forget();
                break;
            case "Heart(Clone)":
                other.gameObject.SetActive(false);
                GameManager.Ist.uiAudio.PlayOneShot(GameManager.Ist.heartAudio);
                GameUI.Ist.AddHp(50).Forget();
                break;
        }
    }
    private async UniTaskVoid MoveInDoor(Collider2D door, Vector3 dir)
    {
        movable = m_Box.enabled = false;
        m_Movement = Vector2.zero;
        var room = door.GetComponentInParent<Room>();
        Debug.Log($"room-i: {room.index}, cur: {GameManager.Ist.curRoom.index}");
        if (room && Math.Abs(room.index - GameManager.Ist.curRoom.index) == 1)
        {
            m_Renderer.color = Color.Lerp(m_Renderer.color, new Color(1, 1, 1, 0), 1);
            await UniTask.WaitForSeconds(0.5f);
            CameraController.instance.ChangeTarget(room.transform);
            transform.position -= dir * 2.8f;
            GameManager.Ist.curRoom = room;
            m_Renderer.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, 1);
            await UniTask.WaitForSeconds(0.5f);
        }
        m_Box.enabled = movable = true;
    }

    private float m_HurtTimer;
    public void TakeDamage(int damage)
    {
        Debug.Log("Player TakeDamage");
        if (m_IsDead) return;
        GameUI.Ist.ChangeHealth(damage);
        if (Time.time - m_HurtTimer > 1)
        {
            m_Animator.SetTrigger(Hurt);
            m_HurtTimer = Time.time;
        }
        if (GameUI.Ist.health < 1) Die().Forget();
    }

    private async UniTaskVoid Die()
    {
        m_IsDead = true;
        GameManager.Ist.uiAudio.PlayOneShot(GameManager.Ist.dieAudio);
        m_Rb.linearVelocity = Vector2.zero;
        m_Animator.SetTrigger(Die1);
        await UniTask.WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        UIManager.ShowBox("You are Died!").Forget();
        await UniTask.WaitForSeconds(1f);
        BackToStartScene();
    }

    void BackToStartScene()
    {
        SceneManager.LoadScene("GameStart");
    }


}
