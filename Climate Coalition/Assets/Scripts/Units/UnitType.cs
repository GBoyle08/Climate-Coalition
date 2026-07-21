using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Type", menuName = "Units/Unit Type")]
public class UnitType : ScriptableObject
    {
        [Header("Basic Info")]
        public string UnitName;

        public Sprite Icon;

        public GameObject Prefab;

        public List<ResourceAmount> Cost;

        [Header("Combat")]
        public int MaxHealth = 100;

        public int MaxActionPoints = 5;

        public int AttackRange = 1;

        public int AttackCost = 2;

        public int Damage = 20;
    }