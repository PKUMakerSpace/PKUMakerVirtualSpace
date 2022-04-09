using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        //public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        public NetworkVariable<Vector3> serverPositionDelta = new NetworkVariable<Vector3>();
        //public Vector3 serverPositionDelta = new Vector3(0,0,0);
        public float speed = 25;

        private float syncDeltaTime = 0.25f;
        private float presentSync = 0f;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                
                //Move();
            }
        }

        public void Move()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                //Position.Value = randomPosition;
            }
            else
            {
                //SubmitPositionRequestServerRpc();
            }
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(Vector3 positionDelta)
        {
            //Position.Value = transform.position;
            serverPositionDelta.Value = positionDelta;
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void Update()
        {
            presentSync += Time.deltaTime;
            if(presentSync > syncDeltaTime)
            {
                presentSync = 0f;
            }
            if (IsOwner && IsClient)
            {
                Vector3 positionDelta = new Vector3(0, 0, 0);
                //Debug.Log("is owner");
                if (Input.GetKey(KeyCode.W))
                {
                    positionDelta += new Vector3(0, 0, 1);
                    //Position. = Position.Value + new Vector3(0, 0, 1) * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    positionDelta += new Vector3(0, 0, -1);
                    //Position.Value = Position.Value + new Vector3(0, 0, -1) * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    positionDelta += new Vector3(-1, 0, 0);
                    //Position.Value = Position.Value + new Vector3(-1, 0, 0) * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    positionDelta += new Vector3(1, 0, 0);
                    //Position.Value = Position.Value + new Vector3(1, 0, 0) * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.J))
                {
                    positionDelta += new Vector3(0, 1, 0);
                    //Position.Value = Position.Value + new Vector3(0, 1, 0) * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.K))
                {
                    positionDelta += new Vector3(0, -1, 0);
                    //Position.Value = Position.Value + new Vector3(0, -1, 0) * Time.deltaTime;
                }
                //transform.position += Time.deltaTime * positionDelta * speed;
                SubmitPositionRequestServerRpc(positionDelta);
            }
            /*else
            {
                transform.position = Position.Value;
            }*/

            if (IsHost || IsServer)
            {
                //Debug.Log("update:" + serverPositionDelta.x + " " + serverPositionDelta.y + " " + serverPositionDelta.z);
                transform.position += Time.deltaTime * serverPositionDelta.Value * speed;
            }
            
        }
    }
}