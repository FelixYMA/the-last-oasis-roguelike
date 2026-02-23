using GamePlay;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Hit = Animator.StringToHash("Hit");
    public BoxCollider2D attackTrigger;
    public bool isBoss, moving, running, isAttack;
    public float moveSpeed;
    public int maxHealth = 30;
    public int score = 20;
    public Vector2 moveDir;
    private int m_CurrentHealth;
    private bool m_PlayerInRange;
    private BoxCollider2D m_Box;
    private Rigidbody2D m_Rb;
    private Animator m_Animator;
    private Slider m_Slider;

    void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_Box = GetComponent<BoxCollider2D>();
        m_Slider = GetComponentInChildren<Slider>();
        if (attackTrigger) attackTrigger.gameObject.SetActive(false);
        if (!isBoss) maxHealth = Random.Range(30 + (int)GameManager.Ist.curScene * 5, 50 + (int)GameManager.Ist.curScene * 10);
        m_CurrentHealth = maxHealth;
        InitPosAndMove();
    }

    private void InitPosAndMove()
    {
        transform.localPosition = isBoss ? new Vector2(Random.Range(-7, 8), Random.Range(-3, 0)) : new Vector2(Random.Range(-7, 8), Random.Range(-3, 4));
        var moveDir1 = new Vector2(transform.localPosition.x > 0 ? 1 : -1, 0);
        var moveDir2 = new Vector2(0, transform.localPosition.y > 0 ? -1 : 1);
        var moveDir3 = new Vector2(transform.localPosition.x > 0 ? -1 : 1, transform.localPosition.y > 0 ? -1 : 1);
        var r = Random.Range(0, 3);
        moveDir = r switch
        {
            0 => moveDir1,
            1 => moveDir2,
            2 => moveDir3,
            _ => moveDir1
        };
    }

    private void FixedUpdate()
    {
        Moving();
        Attacking();
    }
    private void Moving()
    {
        if (!moving || isAttack)
        {
            m_Animator.SetBool(Move, false);
            m_Animator.SetBool(Run, false);
            m_Rb.linearVelocity = Vector2.zero;
            return;
        }
        m_Animator.SetBool(running ? Run : Move, true);
        switch (transform.localPosition.x)
        {
            case >7:
                transform.localScale = m_Slider.transform.localScale = new Vector3(1, 1, 1);
                moveDir = new Vector3(-1, moveDir.y);
                break;
            case <-7:
                transform.localScale = m_Slider.transform.localScale = new Vector3(-1, 1, 1);
                moveDir = new Vector3(1, moveDir.y);
                break;
        }
        moveDir = transform.localPosition.y switch
        {
            > 3 => new Vector3(moveDir.x, -1),
            < -3 => new Vector3(moveDir.x, 1),
            _ => moveDir
        };
        m_Rb.linearVelocity = moveDir * moveSpeed;
    }

    private void Attacking()
    {
        if (m_PlayerInRange && !isAttack)
        {
            isAttack = true;
            m_Animator.SetTrigger(Attack);
            attackTrigger.gameObject.SetActive(true);
            m_Box.enabled = false;
            Invoke(nameof(AttackOver), 0.5f);
        }
    }

    public void AttackOver()
    {
        isAttack = false;
        attackTrigger.gameObject.SetActive(false);
        m_Box.enabled = true;
    }

    public void TakeDamage(int damage)
    {
        m_CurrentHealth -= damage;
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, maxHealth);
        m_Slider.value = (float)m_CurrentHealth / maxHealth;
        if (m_CurrentHealth < 1)
        {
            GameUI.Ist.AddScore(score).Forget();
            m_Animator.SetTrigger(Die);
            m_Slider.fillRect.gameObject.SetActive(false);
            gameObject.SetActive(false);
            if (isBoss)
            {
                // generate chest
                var prefab = Resources.Load<GameObject>($"Prefabs/Props/Chest");
                var ist = Instantiate(prefab, transform.position, Quaternion.identity, transform.parent);
                ist.transform.localPosition = transform.localPosition;
                ist.name = "Chest";
                var exitPrefab = Resources.Load<GameObject>($"Prefabs/Props/Exit");
                var exitIst = Instantiate(exitPrefab, transform.position, Quaternion.identity, transform.parent);
                exitIst.name = "Exit";
                exitIst.transform.localPosition = new Vector3(Random.Range(-7, 8), Random.Range(-3, 4), 0);
            }
            Destroy(gameObject, 0.5f);
        }
        else
        {
            m_Animator.SetTrigger(Hit);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")) m_PlayerInRange = true;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player")) m_PlayerInRange = false;
    }
}
