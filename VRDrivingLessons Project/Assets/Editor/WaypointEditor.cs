/*
  ** This code for waypoints is based off a tutorial by @GameDevGuide on their YouTube video [Building a Traffic System in Unity, 2019]
 */
using UnityEngine;
using UnityEditor;

[InitializeOnLoad()]
public class WaypointEditor
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
    {
        if ((gizmoType & GizmoType.Selected) != 0)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.blue;
        }

        Gizmos.DrawSphere(waypoint.transform.position, 0.5f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(waypoint.transform.position + (waypoint.transform.right * waypoint.width),
            waypoint.transform.position - (waypoint.transform.right * waypoint.width));

        if (waypoint.previousWaypoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 offset = waypoint.transform.right * waypoint.width / 2f;
            Vector3 offsetTo = waypoint.previousWaypoint.transform.right * waypoint.previousWaypoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.previousWaypoint.transform.position + offsetTo);
        }

        if (waypoint.nextWaypoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 offset = waypoint.transform.right * waypoint.width / 2f;
            Vector3 offsetTo = waypoint.nextWaypoint.transform.right * waypoint.nextWaypoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.nextWaypoint.transform.position + offsetTo);
        }
    }
}
