using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiersUI : Singleton<ModifiersUI>
{
    [SerializeField] private GameObject m_ItemPrefab;
    [SerializeField] private Transform m_ItemsContainer;

    private List<DragItem> m_Items = new List<DragItem>();

    private void Start()
    {
        CreateUI();
    }

    private void CreateUI()
    {
        ObstacleType[] startTypes = LevelController.Instance.GetLevel.GetStartModifiers;

        for (int i = 0; i < startTypes.Length; i++)
        {
            CreateNewModifier(startTypes[i]);
        }
    }

    public void CreateNewModifier(ObstacleType _newObs)
    {
        DragItem newItem = Instantiate(m_ItemPrefab, m_ItemsContainer).GetComponent<DragItem>();
        newItem.SetData(_newObs);
        m_Items.Add(newItem);
    }
}
