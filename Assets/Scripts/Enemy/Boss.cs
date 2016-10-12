using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Boss : Enemy {

    [Header("Boss")]
    public string BossName;

    // FSM
    protected FSMSystem Fsm;
    protected GameObject Player;

    // UI
    private GameObject UI_BossStatus;
    private Text UI_TextName;
    private HealthBar UI_HealthBar;

    protected override void Start()
    {
        // from Enemy and Damageable
        base.Start();

        // Initialize FSM
        InitializeFSM();

        // Get player game object
        Player = GameObject.FindWithTag("Player");
        if (Player == null)
            Debug.LogError("Can't find the game object of player.");

        // Get UI
        InitializeUI();
    }

    protected override void FixedUpdate()
    {
        // from Enemy and Damageable
        base.FixedUpdate();

        // FSM
        Fsm.CurrentState.Reason(Player, gameObject);
        Fsm.CurrentState.Act(Player, gameObject);
    }

    public override void applyDamage(int damage)
    {
        base.applyDamage(damage);

        // Update health bar
        if (UI_HealthBar)
            UI_HealthBar.update(health, maxHealth);
    }

    public void SetTransition(FSMSystem.Transition trans)
    {
        // perform transition
        Fsm.PerformTransition(trans);
    }

    public IEnumerator delayFire(Weapon weapon, float time)
    {
        // delay
        yield return new WaitForSeconds(time);

        // fire
        weapon.fire();
    }

    public IEnumerator delayAimFire(Weapon weapon, GameObject obj, float time)
    {
        // delay
        yield return new WaitForSeconds(time);

        // fire
        weapon.aimFire(obj);
    }
    public override void destroy()
    {
        // Disable UI
        UI_BossStatus.SetActive(false);

        // Slow motion
        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        gameManager.StartCoroutine(SlowMotion(0.25f, 0.5f));

        // Base
        base.destroy();
    }

    private void InitializeUI()
    {
        // Get game manager first
        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // Get boss status UI from game manager
        UI_BossStatus = gameManager.UiBossStatus;
        if (UI_BossStatus)
        {
            // Activate
            UI_BossStatus.SetActive(true);

            // Find the rest of UI
            UI_TextName = UI_BossStatus.transform.Find("BossName").GetComponent<Text>();
            UI_HealthBar = UI_BossStatus.transform.Find("BossHealthBar").GetComponent<HealthBar>();

            // Set boss name
            if (UI_TextName)
                UI_TextName.text = BossName;

            // Update health bar
            if (UI_HealthBar)
                UI_HealthBar.update(health, maxHealth);
        }
        else
        {
            Debug.LogError("Can't find the UI for this boss.");
        }
    }

    private IEnumerator SlowMotion(float speed, float duration)
    {
        Time.timeScale = speed;
        yield return new WaitForSeconds(duration);
        Time.timeScale = 1.0f;
    }

    protected abstract void InitializeFSM();
}
