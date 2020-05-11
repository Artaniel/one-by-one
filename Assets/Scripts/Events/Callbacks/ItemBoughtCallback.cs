using System.Collections;
using UnityEngine;

namespace Game.Events.Callbacks
{
    public class ItemBoughtCallback : MonoBehaviour
    {
        [SerializeField] private SkillBase checkSkill = null;

        private SpriteRenderer sprite;
        
        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            sprite = player.transform.GetChild(0).GetComponent<SpriteRenderer>();

            EventManager.OnItemBought.AddListener(OnItemBought);
        }

        public void OnItemBought(SkillBase skill)
        {
            if (skill == checkSkill)
            {
                Color c = sprite.color;
                c.r = 0f;
                sprite.color = c;

                StartCoroutine(RestoreColor());
            }
        }

        IEnumerator RestoreColor()
        {
            yield return new WaitForSeconds(2f);
            Color c = sprite.color;
            c.r = 1f;
            sprite.color = c;
        }
    }
}