using UnityEngine;

namespace Content.Character
{
    public class CharacterData
    {
        public int Id { get; private set; }
        public int Level { get; private set; }
        public string Name { get; private set; }
        public int Hp { get; set; }
        public int MaxHp { get; private set; }
        public int Attack { get; private set; }
        public float AttackReach { get; private set; }
        public float AttackSpeed { get; private set; }

        public CharacterData()
        {
            Level = 4;

            Id = 1;
            Name = "hi";
            AttackReach = 0.5f;
            AttackSpeed = 1;

            MaxHp = 250;
            Hp = MaxHp;
            Attack = 30;
        }
    }
}