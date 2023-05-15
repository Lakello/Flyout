using UnityEngine;
using UnityEngine.Events;
using YG;

[RequireComponent(typeof(CarShop))]
public class YandexDataSaver : MonoBehaviour
{
    [SerializeField] private int _money;
    [SerializeField] private int _lastSelectedCarIndex;
    [SerializeField] private Car[] _cars;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private CarShop _carShop;

    public int Money => _money;
    public int LastSelectedCarIndex => _lastSelectedCarIndex;

    public event UnityAction DataUpdated;

    private void OnValidate()
    {
        if (_wallet == null)
            throw new System.Exception($"� {nameof(YandexDataSaver)} " +
                $"����� �������� {nameof(Wallet)}");

        if (_carShop == null)
            _carShop = GetComponent<CarShop>();
    }

    private void OnEnable() => YandexGame.GetDataEvent += GetLoad;
    private void OnDisable() => YandexGame.GetDataEvent -= GetLoad;

    private void Awake()
    {
        _cars = GetComponentsInChildren<Car>();

        if (YandexGame.SDKEnabled)
            GetLoad();
    }
    
    public bool GetIsBuyedCarByIndex(int index)
    {
        return _cars[index].IsBuyed;
    }

    public void Save()
    {
        YandexGame.savesData.Money = _wallet.Money;
        YandexGame.savesData.LastSelectedCarIndex = _carShop.LastSelectedCarIndex;

        bool[] temp = new bool[_cars.Length];
        for (int i = 0; i < _cars.Length; i++)
            temp[i] = _cars[i].IsBuyed;

        YandexGame.savesData.BuyedCar = temp;

        YandexGame.SaveProgress();
    }

    private void GetLoad()
    {
        _money = YandexGame.savesData.Money;

        _lastSelectedCarIndex = YandexGame.savesData.LastSelectedCarIndex;

        for (int i = 0; i < YandexGame.savesData.BuyedCar.Length; i++)
        {
            bool isBuyed = YandexGame.savesData.BuyedCar[i];
            if (isBuyed)
                _cars[i].Purchase();
        }

        Debug.Log($"Language - {YandexGame.savesData.language}\n" +
            $"First Session - {YandexGame.savesData.isFirstSession}\n" +
            $"Prompt Done - {YandexGame.savesData.promptDone}\n");

        DataUpdated?.Invoke();
    }
}
