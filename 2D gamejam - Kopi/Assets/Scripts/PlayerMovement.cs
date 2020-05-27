using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Rigidbody2D rb;
    public Animator animator;

    public Vector2 position;
    Vector2 movement;
    Vector2 originalPosition;

    public int index;
    private int currentSceneIndex;

    public WorldMusic worldMusic;

    private void Start()
    {
        //LoadData();
        originalPosition.x = 0f;
        originalPosition.y = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if(animator.GetFloat("Horizontal") == -1 || animator.GetFloat("Horizontal") == 1 || animator.GetFloat("Vertical") == -1 || animator.GetFloat("Vertical") == 1){
            animator.SetFloat("LastMoveHorizontal", animator.GetFloat("Horizontal"));
            animator.SetFloat("LastMoveVertical", animator.GetFloat("Vertical"));
        }

        position = transform.position;

        if (Input.GetKey(KeyCode.Escape))
        {
            GoToMainMenu();
        }
    }

    void FixedUpdate()
    {
        //Movement
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    /* 
     * On triggerend 2D collider, it checks if the other gameobject it collided with has been challenged before, and if it's an enemy.
     * It then sends over the current scene number to the battle scene aswell as the amount of enemies.
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<EnterBattle>())
        {
            EnterBattle enemy = other.gameObject.GetComponent<EnterBattle>();
            if (!enemy.hasBeenChallenged)
            {
                int enemyAmount = enemy.amountOfEnemies;
                if (other.CompareTag("Enemy"))
                {
                    enemy.hasBeenChallenged = true;
                    currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                    PlayerPrefs.SetInt("SavedScene", currentSceneIndex);
                    PlayerPrefs.SetInt("AmountOfEnemies", enemyAmount);

                    SaveData(this, enemy);

                    //Load level with build index
                    SceneManager.LoadScene(index);
                }
            }
        }
    }

    public void SaveData(PlayerMovement player, EnterBattle enemy)
    {
        SaveSystem.SaveScene(player, enemy);
    }

    /*public void LoadData()
    {
        UnitData data = SaveSystem.LoadScene();
        Vector2 position;
        position.x = data.position[0];
        position.y = data.position[1];

        transform.position = position;

        if(PlayerPrefs.GetInt("Lost") == 1)
        {
            GameObject enemy = FindClosestEnemy();
            enemy.GetComponent<EnterBattle>().hasBeenChallenged = false;
        }
        
    }*/

    public GameObject FindClosestEnemy()
    {
        GameObject[] enemy;
        enemy = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in enemy)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    public void ResetData()
    {
        transform.position = originalPosition;

        GameObject enemy = FindClosestEnemy();
        enemy.GetComponent<EnterBattle>().hasBeenChallenged = false;

        SaveData(this, enemy.GetComponent<EnterBattle>());

        worldMusic.worldMusic.Play(0);
        worldMusic.worldMusic.PlayDelayed(1);
    }

    void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
