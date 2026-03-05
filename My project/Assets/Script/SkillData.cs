using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public float cooldown = 1f;

    public void Activate(GameObject player)
    {
        Debug.Log("Skill dipakai: " + skillName);
    }
}
