using UnityEngine;
using TMPro;

public class SelectUIManager : MonoBehaviour
{
    // sigen
    [SerializeField]private TextMeshProUGUI gemText;
    [SerializeField]private TextMeshProUGUI coinText;

    // Upgrade Panel
    [SerializeField]private GameObject upgradePanel;
    [SerializeField]private GameObject gotoUpgradePanel;
    [SerializeField]private GameObject casinoUpgradePanel;
    [SerializeField]private GameObject escapeUpgradePanel;


    [SerializeField]private TextMeshProUGUI[] goutouTopics;
    [SerializeField]private TextMeshProUGUI[] casinoTopics;
    [SerializeField]private TextMeshProUGUI[] escapeTopics;

    [SerializeField]private TextMeshProUGUI[] goutouCosts;
    [SerializeField]private TextMeshProUGUI[] casinoCosts;
    [SerializeField]private TextMeshProUGUI[] escapeCosts;



    [SerializeField]private PlayerData playerData;
    [SerializeField]private GoutouData goutouData;
    //[SerializeField]private CasinoData casinoData;
    [SerializeField]private EscapeData escapeData;

    void Update()
    {
        gemText.text=$"{playerData.gem}";
        coinText.text=$"{playerData.coin}";

        //goutou用panelのテキストを更新
        goutouTopics[0].text=$"移動速度 LV.{goutouData.moveSpeedLevel}";
        goutouTopics[1].text=$"収容能力 LV.{goutouData.maxItemLevel}";
        goutouTopics[2].text=$"体力 LV.{goutouData.maxHealthLevel}";
        goutouTopics[3].text=$"攻撃頻度 LV.{goutouData.attackCoolDownLevel}";
        goutouTopics[4].text=$"銃 LV.{goutouData.gunLevel}";

        //casino用panelのテキストを更新

        //escape用panelのテキストを更新
        escapeTopics[0].text=$"ジャンプ力 LV.{escapeData.jumpForceLevel}";
        escapeTopics[1].text=$"ゲーム時間 LV.{escapeData.gameTimeLevel}";
        escapeTopics[2].text=$"二段ジャンプ LV.{escapeData.doubleJumpLevel}";

        // コストのテキストを更新
        goutouCosts[0].text=$"{goutouData.moveSpeedLevel*100+goutouData.moveSpeedLevel*20f}";
        goutouCosts[1].text=$"{goutouData.maxItemLevel*100+goutouData.maxItemLevel*20f}";
        goutouCosts[2].text=$"{goutouData.maxHealthLevel*100+goutouData.maxHealthLevel*20f}";
        goutouCosts[3].text=$"{goutouData.attackCoolDownLevel*100+goutouData.attackCoolDownLevel*20f}";
        goutouCosts[4].text=$"{goutouData.gunLevel*100+goutouData.gunLevel*20f}";

        // casinoCosts[0].text=$"{casinoData.slotMachineCost}";
        // casinoCosts[1].text=$"{casinoData.blackjackCost}";
        // casinoCosts[2].text=$"{casinoData.rouletteCost}";

        escapeCosts[0].text=$"{escapeData.jumpForceLevel*100+escapeData.jumpForceLevel*20f}";
        escapeCosts[1].text=$"{escapeData.gameTimeLevel*100+escapeData.gameTimeLevel*20f}";
        escapeCosts[2].text=$"{escapeData.doubleJumpLevel*100+escapeData.doubleJumpLevel*20f}";
    }


    private bool BuyUpgrade(int currentLevel, System.Action upgradeAction)
    {
        int cost = currentLevel * 100 + (currentLevel - 1) * 20;

        if (playerData.coin < cost)
            return false;

        playerData.coin -= cost;
        upgradeAction();

        return true;
    }

    public void OnClickUpgradeMoveSpeed()
    {
        BuyUpgrade(goutouData.moveSpeedLevel,() => goutouData.moveSpeedLevel++);
    }

    public void OnClickUpgradeMaxItem()
    {
        BuyUpgrade(
            goutouData.maxItemLevel,
            () => goutouData.maxItemLevel++
        );
    }

    public void OnClickUpgradeMaxHealth()
    {
        BuyUpgrade(
            goutouData.maxHealthLevel,
            () => goutouData.maxHealthLevel++
        );
    }

    public void OnClickUpgradeAttackCoolDown()
    {
        BuyUpgrade(
            goutouData.attackCoolDownLevel,
            () => goutouData.attackCoolDownLevel++
        );
    }

    public void OnClickUpgradeGun()
    {
        if (goutouData.gunLevel >= 4)
        {
            Debug.Log("Gun is already at max level.");
            return;
        }
        BuyUpgrade(
            goutouData.gunLevel,
            () => goutouData.gunLevel++
        );
    }

    public void OnClickUpgradeJumpForce()
    {
        BuyUpgrade(
            escapeData.jumpForceLevel,
            () => escapeData.jumpForceLevel++
        );
    }

    public void OnClickUpgradeGameTime()
    {
        BuyUpgrade(
            escapeData.gameTimeLevel,
            () => escapeData.gameTimeLevel++
        );
    }

    public void OnClickUpgradeDoubleJump()
    {
        BuyUpgrade(
            escapeData.doubleJumpLevel,
            () => escapeData.doubleJumpLevel++
        );
    }

    public void OnClickShowUpgradeButton()
    {
        upgradePanel.SetActive(true);
    }

    public void OnClickCloseUpgradeButton()
    {
        upgradePanel.SetActive(false);
    }

    public void OnClickgotoToggle()
    {
        gotoUpgradePanel.SetActive(true);
        casinoUpgradePanel.SetActive(false);
        escapeUpgradePanel.SetActive(false);
    }

    public void OnClickCasinoToggle()
    {
        gotoUpgradePanel.SetActive(false);
        casinoUpgradePanel.SetActive(true);
        escapeUpgradePanel.SetActive(false);
    }

    public void OnClickEscapeToggle()
    {
        gotoUpgradePanel.SetActive(false);
        casinoUpgradePanel.SetActive(false);
        escapeUpgradePanel.SetActive(true);
    }


}