using UnityEngine;
using UnityEngine.EventSystems;

public class HiddenButton : MonoBehaviour, IPointerDownHandler
{ 
    [SerializeField] private float _timeClick = 1f;
    [SerializeField] private int _eventClick = 5;

    private Game _game;

    private float _timeDown;
    private float _timeDownOld;

    private int _clickCount;

    private void Start()
    {
        _game = Game.InstanceF;  
    }

    private void OnEnable()
    {
        _clickCount = 0;
        _timeDown = 0;
        _timeDownOld = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _timeDownOld = _timeDown;
        _timeDown = Time.realtimeSinceStartup;

        if ((_timeDown - _timeDownOld) < _timeClick)
            _clickCount++;
        else
            _clickCount = 1;

        if (_clickCount == _eventClick) 
        {
            _game.LevelCompletedCheat();
        }
    }

}
