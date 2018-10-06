using Byn.Net;
using System.Linq;
using UnityEngine;

class Functions
{

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask layerMask = (1 << Layers.Ground);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            if (hit.collider.tag == Tags.Ground)
            {
                return hit.point;

            }
        return Vector3.zero;
    }

    //Equation of the line : y = slope * x + constant
    public static float DistanceFromLine(float slope, float constant, float x, float y)
    {
        return Mathf.Abs(slope * x - y + constant) / Mathf.Sqrt(slope * slope + 1);
    }

    public static Vector3 GetRandomPointInCube(GameObject cube)
    {
        Vector3 min = Vector3.Scale(cube.GetComponent<MeshFilter>().mesh.bounds.min, cube.transform.localScale);
        Vector3 max = Vector3.Scale(cube.GetComponent<MeshFilter>().mesh.bounds.max, cube.transform.localScale);
        return GetRandomPointInVolume(cube.transform.position, min, max);
    }

    public static Vector3 GetRandomPointInVolume(Vector3 origin, Vector3 min, Vector3 max)
    {
        return origin - new Vector3((Random.Range(min.x, max.x)),
                    (Random.Range(min.y, max.y)),
                    (Random.Range(min.z, max.z)));
    }

    public static void SetLayer(Transform transform, int layer)
    {
        if (transform.gameObject.tag != Tags.Ball)
        {
            transform.gameObject.layer = layer;
            foreach (Transform t in transform) { SetLayer(t, layer); };
        }
    }

    private static System.Random random = new System.Random();

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static bool IsDevelopperComboPressed()
    {
        return Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow);
    }

    public static GameObject InstantiatePlayer(ConnectionId id)
    {
        return MyComponents.NetworkViewsManagement.Instantiate("MyPlayer", new Vector3(0, 4.4f, 0), Quaternion.identity, id);
    }

    public static Vector3 GetInputDirection()
    {
        KeyCode[] movementKeys = UserSettings.MovementKeys;
        return new MovementInputPacket(
                    MyComponents.InputManager.GetKey(movementKeys[0]),
                    MyComponents.InputManager.GetKey(movementKeys[1]),
                    MyComponents.InputManager.GetKey(movementKeys[2]),
                    MyComponents.InputManager.GetKey(movementKeys[3])
                ).GetDirection();
    }
}

