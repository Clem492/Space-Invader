using UnityEngine;
using UnityEngine.InputSystem;

public class MissilManager : MonoBehaviour
{
    [SerializeField]
    private GameObject missilePrefab;

    [SerializeField]
    private Transform firePoint;

    public int poolSize = 1;
    private GameObject[] missilePool;
    private int currentMissileIndex = 0; //permet de commencer la recherce à cette index 

    private InputSystem_Actions controls;
    public UFOManager uFOManager;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Player.Fire.performed += ctx => onFire(ctx);
    }

    private void Start()
    {
        missilePool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            missilePool[i] = Instantiate(missilePrefab);
            missilePool[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void onFire (InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !GameManager.Instance.IsPaused && !GameManager.Instance.isExploding)
        {
            //vérifier si un missile incactif est disponible.
            for (int i = 0; i < poolSize; i++)
            {
                int index = (currentMissileIndex +i) % poolSize;

                if (!missilePool[index].activeSelf)
                {
                    missilePool[index].transform.position = firePoint.position;
                    missilePool[index].transform.rotation = firePoint.rotation;
                    missilePool[index].SetActive(true);
                    currentMissileIndex = (index + 1) % poolSize;
                    uFOManager.compteurTirUfo++;
                    if (GameManager.Instance.ufoActive)
                    {
                        uFOManager.compteurUfoScore++;
                    }
                    return; //sortir après avoir trouvé un missile 
                }
            }

            Debug.Log("?? Aucun missile dispobible !");
        }

    }

 
}
