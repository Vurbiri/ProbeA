using TMPro;
using UnityEngine;

public class HelpEnemies : MonoBehaviour
{
    [SerializeField] private TMP_Text _textEnemyOrange;
    [SerializeField] private EnemyShooting _enemyOrange;
    [Space]
    [SerializeField] private TMP_Text _textEnemyRed;
    [SerializeField] private EnemyShooting _enemyRed;
    [Space]
    [SerializeField] private TMP_Text _textEnemyGreen;
    [SerializeField] private EnemyShooting _enemyGreen;
    [Space]
    [SerializeField] private TMP_Text _textEnemyGravity;
    [SerializeField] private Enemy _enemyGravity;
    [Space]
    [SerializeField] private string _keySpeed = "Speed";
    [SerializeField] private string _keyDamage = "Damage";
    [SerializeField] private string _keyRangeAttack = "RangeAttack";
    [SerializeField] private string _keyMaxBullets = "MaxBullets";
    [SerializeField] private string _keySpawnBulletsTime = "SpawnBulletsTime";
    [SerializeField] private string _keyDebuff = "Debuff";
    [SerializeField] private string _keyGravity = "Gravity";

    private Localization _localization;
    private PlayerStates _states;

    private void Awake()
    {
        _localization = Localization.InstanceF;
        _states = PlayerStates.InstanceF;
    }

    private void OnEnable()
    {
        string speed = _localization.GetText(_keySpeed);
        string damage = _localization.GetText(_keyDamage);
        string rangeAttack = _localization.GetText(_keyRangeAttack);
        string maxBullets = _localization.GetText(_keyMaxBullets);
        string spawnBulletsTime = _localization.GetText(_keySpawnBulletsTime);

        _textEnemyOrange.text = SetupEnemyShooting(_enemyOrange);
        _textEnemyRed.text = SetupEnemyShooting(_enemyRed);
        _textEnemyGreen.text = SetupEnemyShooting(_enemyGreen, true);
        _textEnemyGravity.text = SetupEnemyGravity();

        #region Local Functions
        string SetupEnemyShooting(EnemyShooting enemyShooting, bool isDebuff = false)
        {
            string str = speed + ": " + enemyShooting.Speed.ToString();
            str += "\n";
            str += damage + ": " + enemyShooting.Damage.ToString();
            if(isDebuff)
                str += " (" + string.Format(_localization.GetText(_keyDebuff), 100 * (1 - _states.Debuff)) + ")";
            str += "\n";
            str += rangeAttack + ": " + enemyShooting.RangeAttack.ToString();
            str += "\n";
            str += maxBullets + ": " + enemyShooting.MaxBullets.ToString();
            str += "\n";
            str += spawnBulletsTime + ": " + enemyShooting.SpawnBulletsTime.ToString();

            return str;
        }
        
        string SetupEnemyGravity()
        {
            string str = speed + ": " + _enemyGravity.Speed.ToString();
            str += "\n";
            str += rangeAttack + ": " + _enemyGravity.RangeAttack.ToString();
            str += "\n";
            str += "\n";
            str += _localization.GetText(_keyGravity);

            return str;
        }
        #endregion
    }
}
