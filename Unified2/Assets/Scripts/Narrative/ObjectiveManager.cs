using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] GameObject _popUpObjective;
    [SerializeField] Text _objectiveText;

    public void DisplayObjective(string objective)
    {
        if (objective == null)
        {
            _popUpObjective.SetActive(false);
        }
        else
        {
            _popUpObjective.SetActive(true);
            _objectiveText.text = objective;
        }
    }
}
