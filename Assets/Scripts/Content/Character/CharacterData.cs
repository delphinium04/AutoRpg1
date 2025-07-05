using Datatable;
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

        public CharacterData(CharacterInit init)
        {
            var spec = init.Data;
            Level = init.Level;

            Id = spec.Id;
            Name = spec.Name;
            AttackReach = spec.AttackReach;
            AttackSpeed = spec.AttackSpeed;

            MaxHp = spec.HpDefault * (int)Mathf.Pow(spec.HpGrowth, Level);
            Hp = MaxHp;
            Attack = spec.AttackDefault * (int)Mathf.Pow(spec.AttackGrowth, Level);
        }
    }
}