using System;
using Cysharp.Threading.Tasks;
using GamePlay;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;

public class BossL5 : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Hit = Animator.StringToHash("Hit");
    public bool moving, running, isAttack;
    public float moveSpeed;
    public int maxHealth = 30;
    public int currentHealth;
    public int score = 20;
    public Vector2 moveDir;
    private bool m_PlayerInRange;
    private BoxCollider2D m_Box;
    private Rigidbody2D m_Rb;
    private Animator m_Animator;
    private Slider m_Slider;
    private Room m_Room;
    private float m_AttackTime;
    private float m_Timer;

    void Start()
    {
        m_Room = GetComponentInParent<Room>();
        m_Rb = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_Box = GetComponent<BoxCollider2D>();
        m_Slider = GetComponentInChildren<Slider>();
        maxHealth = Random.Range(30 + (int)GameManager.Ist.curScene * 5, 50 + (int)GameManager.Ist.curScene * 10);
        currentHealth = maxHealth;
        m_AttackTime = 1.0f;
        m_Timer = Time.time;
        InitPosAndMove();
    }

    private void InitPosAndMove()
    {
        transform.localPosition = new Vector2(Random.Range(-7, 8), Random.Range(-3, 4));
        moveDir = Vector2.zero;
    }

    private void FixedUpdate()
    {
        m_Room = GetComponentInParent<Room>();
        if (currentHealth < 1) return;
        Moving();
        Attacking();
    }
    private void Moving()
    {
        if (m_Room && GameManager.Ist.curRoom == m_Room && GameManager.Ist.player)
        {
            switch (transform.position.x.CompareTo(GameManager.Ist.player.transform.position.x))
            {
                case >0:
                    RotateYTo(180).Forget();
                    break;
                case <0:
                    RotateYTo(0).Forget();
                    break;
            }
        }
        if (!m_Room || GameManager.Ist.curRoom != m_Room || !moving || m_PlayerInRange || isAttack)
        {
            m_Animator.SetBool(Move, false);
            m_Animator.SetBool(Run, false);
            m_Rb.linearVelocity = Vector2.zero;
            return;
        }
        m_Animator.SetBool(running ? Run : Move, true);
        m_Rb.linearVelocity = moveDir * moveSpeed;
    }

    private Vector2 m_TempDir;
    private async UniTaskVoid RotateYTo(float angle)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        if (currentHealth < 1 || !transform || !m_Slider || !GameManager.Ist.player) return;
        transform.localRotation = m_Slider.transform.localRotation = Quaternion.Euler(0, angle, 0);
        m_TempDir = (GameManager.Ist.player.transform.position - transform.position).normalized;
        if (moveDir == Vector2.zero || !moveDir.Equals(m_TempDir)) moveDir = m_TempDir;
    }
    private void Attacking()
    {
        if (m_PlayerInRange && !isAttack && Time.time - m_Timer > m_AttackTime)
        {
            m_Timer = Time.time;
            isAttack = true;
            m_Animator.SetTrigger(Attack);
            m_Box.enabled = false;
            Invoke(nameof(AttackOver), 0.5f);
        }
    }

    public void AttackOver()
    {
        isAttack = false;
        m_Box.enabled = true;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"Enemy-{name}, TakeDamage!");
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        m_Slider.value = (float)currentHealth / maxHealth;
        if (currentHealth < 1)
        {
            GameUI.Ist.AddScore(score).Forget();
            m_Animator.SetTrigger(Die);
            m_Slider.fillRect.gameObject.SetActive(false);
            gameObject.SetActive(false);
            // generate chest
            var prefab = Resources.Load<GameObject>($"Prefabs/Props/Chest");
            var ist = Instantiate(prefab, transform.position, Quaternion.identity, transform.parent);
            ist.transform.localPosition = transform.localPosition;
            ist.name = "Chest";
            var exitPrefab = Resources.Load<GameObject>($"Prefabs/Props/Exit");
            var exitIst = Instantiate(exitPrefab, transform.position, Quaternion.identity, transform.parent);
            exitIst.name = "Exit";
            exitIst.transform.localPosition = new Vector3(Random.Range(-5, 6), Random.Range(-2, 3), 0);
            Destroy(gameObject, 0.5f);
        }
        else
        {
            m_Animator.SetTrigger(Hit);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            m_Animator.SetBool(Move, false);
            m_Animator.SetBool(Run, false);
            m_Rb.linearVelocity = Vector2.zero;
            m_PlayerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) m_PlayerInRange = false;
    }
}
