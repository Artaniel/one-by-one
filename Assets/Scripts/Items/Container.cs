using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Container : MonoBehaviour
{
    public int itemListSize = 0;
    public AudioClip containerOpened = null;
    [HideInInspector] public GameObject[] itemList;
    [HideInInspector] public float[] itemChances;
    private GameObject itemToDrop = null;
    [HideInInspector] public RoomBlueprint blueprint;
    private SkillManager playerSkillManager;

    protected virtual void Start()
    {
        playerSkillManager = GameObject.FindWithTag("Player").GetComponent<SkillManager>();
        //GetItem();
    }

    private void GetItem()
    {
        if (itemList.Length > 0) // exception for empty list
        {
            float summ = 0;
            foreach (float p in itemChances)
            {
                summ += p;
            }
            if (summ > 0) // exception for 0 chances for all items
            {
                if (LabirintBuilder.seed != "") {
                    Random.InitState(LabirintBuilder.seed.GetHashCode() + transform.position.GetHashCode());
                }
                float random = Random.Range(0f, summ);
                int i = 0;
                while (random > 0)
                {
                    random -= itemChances[i];
                    i++;
                }
                itemToDrop = itemList[i - 1];
                if (DuplicateCheck(itemToDrop))
                {
                    itemChances = DeleteFromArray(itemChances, i-1);
                    itemList = DeleteFromArrayGameObject(itemList, i-1);
                    itemToDrop = null;
                    GetItem(); //рекурсия чтобы повторить после удаления, надо чтобы правильно пересчитать вероятности без удаленного итема
                }
            }
        }
        else {
            Debug.Log("Empty Drop List. Maybe after duplicate exclution.");
        }
    }

    bool DuplicateCheck(GameObject itemToDrop) {
        bool result = false;
        if (itemToDrop != null) // for empty list
            if (itemToDrop.GetComponent<PickupableSkill>() != null) // for non-skill items
            {
                string SkillToDropName = itemToDrop.GetComponent<PickupableSkill>().skill.name + "(Clone)";
                foreach (SkillBase skill in playerSkillManager.skills)
                {
                    if (skill.name == SkillToDropName)
                    {
                        result = true;
                    }
                }
            }
        return result;
    }

    float[] DeleteFromArray(float[] array, int indexToDelete)
    {
        float[] result = new float[array.Length - 1];
        for (int i = 0; i < array.Length - 1; i++)
        {
            if (i < indexToDelete)
                result[i] = array[i];
            else
                result[i] = array[i + 1];
        }
        return result;
    }

    GameObject[] DeleteFromArrayGameObject(GameObject[] array, int indexToDelete)
    {        
        GameObject[] result = new GameObject[array.Length - 1];
        for (int i = 0; i < array.Length - 1; i++)
        {
            if (i < indexToDelete)
                result[i] = array[i];
            else
                result[i] = array[i + 1];
        }
        return result;
    }

    public void Open()
    {
        if (containerOpened) AudioManager.Play(containerOpened);
        GetItem();
        if (itemToDrop != null)
            Instantiate(itemToDrop, transform.position, transform.rotation);
        else
        {
            //Debug.Log("Error on container open. Empty drop list");
            //null drop is possible now, for not 100% mob drop, so not a error now
        }
        if (blueprint != null)
        {
            blueprint.containerWasOpened = true;
        }
    }

#if UNITY_EDITOR
    public static void Table(Container container) // for inspecrot UI
    {
        GUILayout.BeginHorizontal(); // table headline
        GUILayout.Label("Prefab", GUILayout.Width(120));
        GUILayout.Label("Chance", GUILayout.Width(50));
        GUILayout.Label("%", GUILayout.Width(50));
        GUILayout.EndHorizontal();

        float[] lastChances = container.itemChances;
        GameObject[] lastItemList = container.itemList;
        if (lastChances.Length != container.itemListSize)
        { //if array size changed, make new arrays and move data to them            
            container.itemChances = new float[container.itemListSize];
            container.itemList = new GameObject[container.itemListSize];
            for (int i = 0; i < Mathf.Min(lastChances.Length, container.itemChances.Length); i++)
            {
                container.itemChances[i] = lastChances[i];
                container.itemList[i] = lastItemList[i];
            }
            EditorUtility.SetDirty(container); // to prevent load from prefab on Play    
        }

        float summ = 0; // summ for % output
        foreach (float p in container.itemChances)
        {
            summ += p;
        }
        for (int i = 0; i < container.itemListSize; i++)
        {
            GUILayout.BeginHorizontal();
            {
                GameObject lastItem = container.itemList[i];
                container.itemList[i] = (GameObject)EditorGUILayout.ObjectField(container.itemList[i], typeof(GameObject), false, GUILayout.Width(120));
                if (lastItem != container.itemList[i])
                    EditorUtility.SetDirty(container);

                string chance = container.itemChances[i].ToString();
                chance = EditorGUILayout.TextField("", chance, GUILayout.Width(50));
                if (chance != container.itemChances[i].ToString()) // if changed
                    EditorUtility.SetDirty(container);
                float.TryParse(chance, out container.itemChances[i]);

                EditorGUILayout.LabelField((container.itemChances[i] * 100f / summ).ToString() + "%");
            }
            GUILayout.EndHorizontal();
        }
    }
#endif
}
