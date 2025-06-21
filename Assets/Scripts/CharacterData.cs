using UnityEngine;

[CreateAssetMenu(fileName ="Character", menuName ="SO/CharacterData")]
public class CharacterData: ScriptableObject
{
    public int Hp => hp;
    public int Atk => attack;
    public int Defense => defense;
    public int CriticalProbabilityRate => criticalProbabilityRate;
    public int CriticalDamageRate => criticalDamageRate;

    [SerializeField] private int hp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int criticalProbabilityRate; // 0 ~ 100
    [SerializeField] private int criticalDamageRate; // 0 ~
}
