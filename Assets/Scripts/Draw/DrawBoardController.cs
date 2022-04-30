using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

namespace PKU.Draw
{
    public class DrawBoardController : NetworkBehaviour
    {
        public DrawCanvasController drawCanvas;
        
        public GameObject drawCamera;

        public bool isInUsed = false;

        public float interactRadius;

        public List<PlayerDraw> playerInRadius = new List<PlayerDraw>();

        public List<PlayerDraw> playerLastFrame = new List<PlayerDraw>();

        // Start is called before the first frame update
        void Start()
        {
            drawCamera.SetActive(false);
        }

        private void Update()
        {
            FindPlayersInRadius();
        }

        private void FindPlayersInRadius()
        {

            playerInRadius.Clear();

            Collider[] colls = Physics.OverlapSphere(this.transform.position, interactRadius, 1 << LayerMask.NameToLayer("Player"));

            if (colls.Length > 0)
            {
                for (int i = 0; i < colls.Length; i++)
                {
                    // Debug.Log("检测到玩家");
                    GameObject player = colls[i].gameObject;

                    PlayerDraw playerDraw = player.GetComponent<PlayerDraw>();

                    if (playerDraw != null)
                    {
                        playerInRadius.Add(playerDraw);

                        playerDraw.drawBoard = this;
                    }

                }
            }

            playerLastFrame = playerInRadius;

            /*var diff = playerLastFrame.Where(x => !playerInRadius.Any(a => x == a)).ToList();

            foreach (var pd in diff)
            {
                pd.drawBoard = null;
            }

            */

        }

    }
}
