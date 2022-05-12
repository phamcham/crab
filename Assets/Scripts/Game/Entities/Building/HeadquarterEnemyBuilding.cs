using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadquarterEnemyBuilding : Entity, IDamagable {
    public override Team Team => Team.DefaultEnemy;
    [SerializeField] EnemyBuildingProperties properties;
    [SerializeField] BubbleEnemyCrabUnit bubbleEnemyCrabUnitPrefab;
    [SerializeField] HealthBar healthBarEnemy;
    private void Start() {
        properties.area = GridBuildingSystem.current.CalculateAreaFromWorldPosition(properties.area, transform.position);
        GridBuildingSystem.current.TakeArea(properties.area);

        healthBarEnemy.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        healthBarEnemy.Hide();
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
}

[System.Serializable]
public struct EnemyBuildingProperties {
    public int maxHealthPoint;
    [HideInInspector]
    public int curHealthPoint;
    public BoundsInt area;
}