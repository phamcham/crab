using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadquarterEnemyBuilding : EnemyBuilding, IDamagable, ISaveObject<EnemyHeadquarterBuildingSaveData> {
    public override BuildingType type => BuildingType.Headquarter;
    [SerializeField] BubbleEnemyCrabUnit bubbleEnemyCrabUnitPrefab;
    [SerializeField] HealthBar healthBarEnemy;
    private void Start() {
        properties.curHealthPoint = properties.maxHealthPoint;
        healthBarEnemy.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        healthBarEnemy.Hide();

        if (CanBePlaced()) {
            Place();
        }
    }
    [ContextMenu("Spawn enemies")]
    private void SpawnEnemies() {
        int n = Random.Range(3, 5);
        for (int i = 0; i < n; i++) {
            Vector3 position = transform.position + Random.insideUnitSphere * 5;
            position.z = 0;
            BubbleEnemyCrabUnit crab = Instantiate(bubbleEnemyCrabUnitPrefab, position, Quaternion.identity);
            crab.SetTarget(Random.value > 0.2f);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            SpawnEnemies();
        }
    }

    public void TakeDamage(int damage) {
        int curHeath = properties.curHealthPoint;
        int maxHeath = properties.maxHealthPoint;
        //print("cuerh: " + curHeath + "/" + maxHeath);
        curHeath -= damage;
        if (curHeath < 0) curHeath = 0;
        properties.curHealthPoint = curHeath;

        healthBarEnemy.SetSize(1.0f * curHeath / maxHeath);

        if (curHeath == 0) {
            Destroy(gameObject);
        }
    }

    public override void OnBuildingPlaced() {
        
    }

    protected override void OnDestroyBuilding() { }

    public EnemyHeadquarterBuildingSaveData GetSaveObjectData() {
        return new EnemyHeadquarterBuildingSaveData() {
            building = new BuildingSaveData() {
                maxHealthPoint = properties.maxHealthPoint,
                curHealthPoint = properties.curHealthPoint,
                position = new SaveSystemExtension.Vector2(transform.position)
            }
        };
    }
}

[System.Serializable]
public struct EnemyBuildingProperties {
    public int maxHealthPoint;
    [HideInInspector]
    public int curHealthPoint;
    public BoundsInt area;
}

[System.Serializable]
public struct EnemyHeadquarterBuildingSaveData {
    public BuildingSaveData building;
}