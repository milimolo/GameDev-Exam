using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    //enemies special attacks
    public GameObject enemyJotaroSpecialAttack;
    public GameObject enemyDioSpecialAttack;
    public GameObject enemyHeal;
    //allied special attacks
    public GameObject dioSpecialAttack;
    public GameObject jotaroSpecialAttack;
    public GameObject playerHeal;

    GameObject enemyGO;
    GameObject playerGO;

    public GameObject[] friendlyPrefabs = new GameObject[2];
    public GameObject[] enemyPrefabs = new GameObject[2];

    Unit playerUnit;
    Unit enemyUnit;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;
    private int sceneToContinue = 3;
    private int gameOverScene = 4;

    public Animator playerAnimator;
    public Animator enemyAnimator;

    AudioSource audioData;
    public Text specialButton;
    public Text healButton;
    public Text amountOfSpecialLeft;

    private int specialCounter;
    private int enemySpecialCounter;
    private int enemySpecialDamage;

    private int specialMultiplier;
    private int specialDamage;

    private System.Random randomEnemyTeam;
    private int enemyToFight;

    private System.Random randomFriendlyTeam;
    private int friendlyTeam;

    private string enemyUnitName;
    private string playerUnitName;

    int enemiesDefeated;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;

        randomEnemyTeam = new System.Random();
        enemyToFight = randomEnemyTeam.Next(1, 3);

        randomFriendlyTeam = new System.Random();
        friendlyTeam = randomFriendlyTeam.Next(1, 3);

        StartCoroutine(SetupBattle());

        audioData = GetComponent<AudioSource>();

        specialCounter = 2;
        enemySpecialCounter = 2;

        specialMultiplier = 3;
        specialDamage = playerUnit.damage * specialMultiplier;
        enemySpecialDamage = enemyUnit.damage * specialMultiplier;
        amountOfSpecialLeft.text = specialCounter + "/2";
    }

    IEnumerator SetupBattle()
    {
        enemiesDefeated = 0;
        switch (friendlyTeam)
        {
            case 1:
                playerGO = Instantiate(friendlyPrefabs[0], playerBattleStation);
                playerUnit = playerGO.GetComponent<Unit>();
                break;
            case 2:
                playerGO = Instantiate(friendlyPrefabs[1], playerBattleStation);
                playerUnit = playerGO.GetComponent<Unit>();
                break;
            default:
                break;
        }
        
        
        switch (enemyToFight)
        {
            case 1:
                enemyGO = Instantiate(enemyPrefabs[0], enemyBattleStation);
                enemyUnit = enemyGO.GetComponent<Unit>();
                break;
            case 2:
                enemyGO = Instantiate(enemyPrefabs[1], enemyBattleStation);
                enemyUnit = enemyGO.GetComponent<Unit>();
                break;
            default:
                break;
        }

        enemyUnitName = enemyUnit.unitName;
        playerUnitName = playerUnit.unitName;

        dialogueText.text = "Hohoo, " + enemyUnitName + ", you're approaching me...";
        ResetHealthOfTeam();

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2);

        PlayerTurn();
    }

    private void ResetHealthOfTeam()
    {
        foreach (var friendly in friendlyPrefabs)
        {
            Unit unit = friendly.GetComponent<Unit>();
            unit.currentHealth = unit.maxHealth;
        }
    }

    void PlayerTurn()
    {
        state = BattleState.PLAYERTURN;
        dialogueText.text = "Choose an action:";
    }

    public void OnAttackButton()
    {
        if(state != BattleState.PLAYERTURN)
        {
            return;
        }
        StartCoroutine(PlayerAttack());
    }

    public void OnSpecialAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        StartCoroutine(PlayerSpecialAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        StartCoroutine(PlayerHeal());
    }

    IEnumerator PlayerHeal()
    {
        if(specialCounter != 0)
        {
            specialCounter--;
            amountOfSpecialLeft.text = specialCounter + "/2";
            if (specialCounter == 0)
            {
                healButton.color = new Color(255f, 0f, 0f);
                specialButton.color = new Color(255f, 0f, 0f);
                amountOfSpecialLeft.color = new Color(255f, 0f, 0f);
            }
            playerUnit.Heal();
            playerHUD.SetHealth(playerUnit.currentHealth);
            dialogueText.text = playerUnitName + " healed himself for " + playerUnit.healAmount + " damage.";
            playerHeal.SetActive(true);
            playerHeal.GetComponent<Animator>().Play("Heal");
            yield return new WaitForSeconds(1f);
            playerHeal.SetActive(false);
            state = BattleState.ENEMYTURN;
            yield return new WaitForSeconds(1f);

            EnemyTurn();
        }
        else
        {
            dialogueText.text = "You cannot use your special or heal more than twice per battle";
            yield return new WaitForSeconds(1.5f);
            PlayerTurn();
        }

    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.SetHealth(enemyUnit.currentHealth);
        if (isDead)
        {
            CheckIfEnemiesStillLeft();
            yield return new WaitForSeconds(2f);
            EnemyTurn();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            dialogueText.text = enemyUnitName + " took " + playerUnit.damage + " damage.";
            enemyAnimator.SetTrigger("DoDamage");
            audioData.Play(0);
            yield return new WaitForSeconds(2f);
            EnemyTurn();
        }
    }

    IEnumerator PlayerSpecialAttack()
    {
        
        if (specialCounter != 0)
        {
            specialCounter--;
            amountOfSpecialLeft.text = specialCounter + "/2";
            if (specialCounter == 0)
            {
                healButton.color = new Color(255f, 0f, 0f);
                specialButton.color = new Color(255f, 0f, 0f);
                amountOfSpecialLeft.color = new Color(255f, 0f, 0f);

            }
            state = BattleState.ENEMYTURN;
            bool isDead = enemyUnit.TakeDamage(playerUnit.damage * specialMultiplier);

            enemyHUD.SetHealth(enemyUnit.currentHealth);
            if (isDead)
            {
                CheckIfEnemiesStillLeft();
            }
            dialogueText.text = playerUnitName + " unleashes a flurry of attacks and " + enemyUnitName + " took " + specialDamage + " damage.";
            
            if(playerUnitName == "Dio")
            {
                dioSpecialAttack.SetActive(true);
                dioSpecialAttack.GetComponent<Animator>().Play("PlayerDioSpecialAttack");

                enemyAnimator.SetTrigger("DoDamage");
                audioData.Play(0);

                yield return new WaitForSeconds(1f);

                dioSpecialAttack.SetActive(false);
            }
            else if(playerUnitName == "Jotaro")
            {
                jotaroSpecialAttack.SetActive(true);
                jotaroSpecialAttack.GetComponent<Animator>().Play("PlayerJotaroSpecialAttackAnimation");

                enemyAnimator.SetTrigger("DoDamage");
                audioData.Play(0);

                yield return new WaitForSeconds(1f);

                jotaroSpecialAttack.SetActive(false);
            }

            yield return new WaitForSeconds(1f);
            EnemyTurn();
            state = BattleState.ENEMYTURN;
        }
        else
        {
            dialogueText.text = "You cannot use your special or heal more than twice per battle";
            yield return new WaitForSeconds(1.5f);
            PlayerTurn();
        }
    }

    void EnemyTurn()
    {
        System.Random rnd = new System.Random();

        int switchCases = rnd.Next(1, 4);

        switch (switchCases)
        {
            case 1:
                StartCoroutine(EnemyAttackDuringTurn());
                break;
            case 2:
                StartCoroutine(EnemySpecialAttackDuringTurn());
                break;
            case 3:
                StartCoroutine(EnemyHealDuringTurn());
                break;
        }
    }

    IEnumerator CheckIfTeammatesStillLeft()
    {
        int friendliesStillLeft = 0;

        foreach (var friendly in friendlyPrefabs)
        {
            Unit unit = friendly.GetComponent<Unit>();
            if (unit.unitName == playerUnitName)
            {
                unit.currentHealth = 0;
                dialogueText.text = playerUnitName + " has been knocked out.";
                yield return new WaitForSeconds(2f);
                Destroy(playerGO);
            }
        }
        foreach (var friendly in friendlyPrefabs)
        {
            Unit unit = friendly.GetComponent<Unit>();
            if (unit.currentHealth != 0)
            {
                friendliesStillLeft++;
                dialogueText.text = "Guess it is " + unit.unitName + "'s time to shine.";

                playerGO = Instantiate(friendly, playerBattleStation);
                playerUnit = unit;
                playerUnitName = playerUnit.unitName;
                playerHUD.SetHUD(playerUnit);
                yield return new WaitForSeconds(2f);
                PlayerTurn();
            }
        }
        if (friendliesStillLeft == 0)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
    }

        void CheckIfEnemiesStillLeft()
    {
        int totalAmountOfEnemies = PlayerPrefs.GetInt("AmountOfEnemies");
        enemiesDefeated++;

        if (enemiesDefeated == totalAmountOfEnemies)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle());
        }
        if(enemiesDefeated != totalAmountOfEnemies)
        {
            StartCoroutine(handleEnemySwapOut());
        }
    }

    IEnumerator handleEnemySwapOut()
    {
        foreach (var enemy in enemyPrefabs)
        {
            Unit unit = enemy.GetComponent<Unit>();
            if (unit.unitName == enemyUnitName)
            {
                unit.currentHealth = 0;
                dialogueText.text = enemyUnitName + " turned to dust.";
                yield return new WaitForSeconds(2f);
                Destroy(enemyGO);
            }
        }
        foreach (var enemy in enemyPrefabs)
        {
            Unit unit = enemy.GetComponent<Unit>();
            if (unit.currentHealth != 0)
            {
                dialogueText.text = "I will smash you!";

                enemyGO = Instantiate(enemy, enemyBattleStation);
                enemyUnit = unit;
                enemyUnitName = enemyUnit.unitName;
                enemyHUD.SetHUD(enemyUnit);
                yield return new WaitForSeconds(2f);
                state = BattleState.ENEMYTURN;
            }
        }
    }

    IEnumerator EnemyAttackDuringTurn()
    {
        
        dialogueText.text = playerUnitName + " is attacked for " + enemyUnit.damage + " damage.";
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        playerHUD.SetHealth(playerUnit.currentHealth);

        playerAnimator.SetTrigger("TakeDamage");
        audioData.Play(0);
        yield return new WaitForSeconds(2f);
        if (isDead)
        {
            StartCoroutine(CheckIfTeammatesStillLeft());
        }
        else
        {
            PlayerTurn();
        }
    }

    IEnumerator EnemySpecialAttackDuringTurn()
    {
        if (enemySpecialCounter != 0)
        {
            enemySpecialCounter--;
            state = BattleState.PLAYERTURN;
            bool isDead = playerUnit.TakeDamage(enemyUnit.damage * specialMultiplier);

            playerHUD.SetHealth(playerUnit.currentHealth);
            if (isDead)
            {
                StartCoroutine(CheckIfTeammatesStillLeft());
            }
            dialogueText.text = enemyUnitName + " unleashes a flurry of attacks and " + playerUnitName + " took " + enemySpecialDamage + " damage.";
            if(enemyUnitName == "Jotaro")
            {
                enemyJotaroSpecialAttack.SetActive(true);
                enemyJotaroSpecialAttack.GetComponent<Animator>().Play("enemyJotaroSpecialAttackAnimation");
                playerAnimator.SetTrigger("TakeDamage");
                audioData.Play(0);

                yield return new WaitForSeconds(1f);

                enemyJotaroSpecialAttack.SetActive(false);

                yield return new WaitForSeconds(1f);

            } else if(enemyUnitName == "Dio")
            {
                enemyDioSpecialAttack.SetActive(true);
                enemyDioSpecialAttack.GetComponent<Animator>().Play("EnemyDioSpecialAttack");
                playerAnimator.SetTrigger("TakeDamage");
                audioData.Play(0);

                yield return new WaitForSeconds(1f);

                enemyDioSpecialAttack.SetActive(false);

                yield return new WaitForSeconds(1f);
            }
            
            PlayerTurn();
        }
        else
        {
            StartCoroutine(EnemyAttackDuringTurn());
        }
    }

    IEnumerator EnemyHealDuringTurn()
    {
        if (enemySpecialCounter != 0)
        {
            enemySpecialCounter--;
            dialogueText.text = enemyUnitName + " heals himself for " + enemyUnit.healAmount + " damage.";
            enemyHeal.SetActive(true);
            enemyHeal.GetComponent<Animator>().Play("Heal");
            yield return new WaitForSeconds(1f);
            enemyHeal.SetActive(false);
            enemyUnit.Heal();
            enemyHUD.SetHealth(enemyUnit.currentHealth);
            yield return new WaitForSeconds(1f);
            PlayerTurn();
        }
        else
        {
            StartCoroutine(EnemyAttackDuringTurn());
        }
    }

    IEnumerator EndBattle()
    {
        if(state == BattleState.WON)
        {
            dialogueText.text = "You copies never stood a chance.";
            yield return new WaitForSeconds(1.5f);
            ReturnToWorld();
        } else if(state == BattleState.LOST)
        {
            dialogueText.text = "Ugh... we lost.";
            yield return new WaitForSeconds(1.5f);
            GameOverMan();
        }
    }

    void ReturnToWorld()
    {
        PlayerPrefs.SetInt("Lost", 0);
        SceneManager.LoadScene(sceneToContinue);
    }

    void GameOverMan()
    {
        SceneManager.LoadScene(gameOverScene);
    }
}