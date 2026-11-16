using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class TrapUI : MonoBehaviour
{
    [SerializeField] private float trapDamagePercent = 0.1f;

    [SerializeField] private TMP_Text trapDesText;
    [SerializeField] private Button exitbutton;

    public void Init()
    {
        exitbutton.onClick.AddListener(HideUI);
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);

        TrapDamage();
    }

    public void HideUI()
    {
        Managers.Event.NotifyEvent(EEventType.OnNodeCompleted, this, false);
        gameObject.SetActive(false);
    }
    private void TrapDamage()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"함정에 걸려 피해를 입었습니다.");

        foreach (var hero in Managers.Battle.Heroes)
        {
            if (hero == null || hero.UnitStat.IsDie) continue;

            float damage = hero.UnitStat.Hp * trapDamagePercent;
            int damageInt = Mathf.FloorToInt(damage);

            hero.UnitStat.Hp -= damage;
            hero.UnitUI.SetHP(hero.UnitStat.Hp, hero.UnitStat.MAXHP);

            sb.AppendLine($"<color=red>{hero.classType}</color>  -{damageInt}");
            if (hero.UnitStat.Hp <= 0)
            {
                hero.UnitStat.IsDie = true;
                hero.Animator.SetTrigger("IsDie");
                Managers.Battle.IsGameLose();

                sb.AppendLine($"<color=grey>({hero.classType} 쓰러짐)</color>");
            }

            trapDesText.text = sb.ToString();
        }
    }
}
