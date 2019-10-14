using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Shinobu_is_me
{
    public class Break_Mesh
    {
        public class Mesh_Child
        {
            public List<Vector3> vertices = new List<Vector3>();
            public List<Vector3> normals = new List<Vector3>();
            public List<Vector2> uvs = new List<Vector2>();
            public List<int> triangles = new List<int>();
            public List<List<int>> subIndices = new List<List<int>>();

            //斜めの辺を切った時に新しく作った頂点と法線
            public List<Vector3> new_across_vertices = new List<Vector3>();
            //まっすぐな辺を切った時に新しく作った頂点と法線
            public List<Vector3> new_half_vertices = new List<Vector3>();
            public List<Vector3> new_under_vetices = new List<Vector3>();

            public void ClearAll()
            {
                vertices.Clear();
                normals.Clear();
                uvs.Clear();
                triangles.Clear();
                subIndices.Clear();
            }

            /// <summary>
            /// トライアングルとして3頂点を追加
            /// ※ 頂点情報は元のメッシュからコピーする
            /// </summary>
            /// <param name="pos">3つの頂点</param>
            /// <param name="submesh">対象のサブメシュ</param>
            public void AddTriangle(int[] pos, int submesh)
            {
                // triangle index order goes 1,2,3,4....

                // 頂点配列のカウント。随時追加されていくため、ベースとなるindexを定義する。
                // ※ AddTriangleが呼ばれるたびに頂点数は増えていく。
                int base_index = vertices.Count;

                // 対象サブメッシュのインデックスに追加していく
                subIndices[submesh].Add(base_index + 0);
                subIndices[submesh].Add(base_index + 1);
                subIndices[submesh].Add(base_index + 2);

                // 三角形郡の頂点を設定
                triangles.Add(base_index + 0);
                triangles.Add(base_index + 1);
                triangles.Add(base_index + 2);

                // 対象オブジェクトの頂点配列から頂点情報を取得し設定する
                vertices.Add(victim_mesh.vertices[pos[0]]);
                vertices.Add(victim_mesh.vertices[pos[1]]);
                vertices.Add(victim_mesh.vertices[pos[2]]);

                // 同様に、対象オブジェクトの法線配列から法線を取得し設定する
                normals.Add(victim_mesh.normals[pos[0]]);
                normals.Add(victim_mesh.normals[pos[1]]);
                normals.Add(victim_mesh.normals[pos[2]]);

                // 同様に、UVも。
                uvs.Add(victim_mesh.uv[pos[0]]);
                uvs.Add(victim_mesh.uv[pos[1]]);
                uvs.Add(victim_mesh.uv[pos[2]]);
            }


            /// <summary>
            /// トライアングルを追加する
            /// </summary>
            /// <param name="points3">トライアングルを形成する3頂点</param>
            /// <param name="normals3">3頂点の法線</param>
            /// <param name="uvs3">3頂点のUV</param>
            /// <param name="faceNormal">ポリゴンの法線</param>
            /// <param name="submesh">サブメッシュID</param>
            public void AddTriangle(Vector3[] points3, Vector3[] normals3, Vector2[] uvs3, Vector3 faceNormal, int submesh)
            {
                // 引数の3頂点から法線を計算
                Vector3 calculated_normal = Vector3.Cross((points3[1] - points3[0]).normalized, (points3[2] - points3[0]).normalized);

                int p1 = 0;
                int p2 = 1;
                int p3 = 2;

                // 引数で指定された法線と逆だった場合はインデックスの順番を逆順にする（つまり面を裏返す）
                if (Vector3.Dot(calculated_normal, faceNormal) < 0)
                {
                    p1 = 2;
                    p2 = 1;
                    p3 = 0;
                }

                int base_index = vertices.Count;

                subIndices[0].Add(base_index + 0);
                subIndices[0].Add(base_index + 1);
                subIndices[0].Add(base_index + 2);

                triangles.Add(base_index + 0);
                triangles.Add(base_index + 1);
                triangles.Add(base_index + 2);

                vertices.Add(points3[p1]);
                vertices.Add(points3[p2]);
                vertices.Add(points3[p3]);

                normals.Add(normals3[p1]);
                normals.Add(normals3[p2]);
                normals.Add(normals3[p3]);

                uvs.Add(uvs3[p1]);
                uvs.Add(uvs3[p2]);
                uvs.Add(uvs3[p3]);
            }
        }


        //切り分けるオブジェクト
        private static Mesh victim_mesh;

        //切り分けたゲームオブジェクト
        private static Mesh_Child[] victim_child = new Mesh_Child[8];



        //縦と横の真ん中の切り分ける座標
        private static Vector3 Center_pos = new Vector3();

        //private static GameObject victim_obj;


        /// <summary>
        /// 引数で渡されたゲームオブジェクトを4分割します
        /// </summary>
        /// <param name="victim">カットされる被害者</param>
        /// <returns></returns>
        public static GameObject[] Cut(GameObject victim)
        {
            //victim_obj = victim;
            //対象のメッシュを取得
            victim_mesh = victim.GetComponent<MeshFilter>().mesh;


            //メッシュの各種設定の初期化
            for (int i = 0; i < victim_child.Length; i++)
            {
                victim_child[i] = new Mesh_Child();
                victim_child[i].ClearAll();
            }


            //サブメッシュのインデックス格納
            int[] indices;
            //個々のインデックスを一時格納
            int[] indice = new int[3];
            //ポジションを一時格納
            int[] pos = new int[3];

            //中心の位置を取る
            float up_point = float.NegativeInfinity;
            float down_point = float.PositiveInfinity;
            float left_point = float.PositiveInfinity;
            float right_point = float.NegativeInfinity;
            float front_point = float.PositiveInfinity;
            float back_point = float.NegativeInfinity;

            for (int i = 0; i < victim_mesh.subMeshCount; i++)
            {
                indices = victim_mesh.GetIndices(i);
                for (int ind = 0; ind < indices.Length; ind += 3)
                {
                    indice[0] = indices[ind + 0];
                    indice[1] = indices[ind + 1];
                    indice[2] = indices[ind + 2];
                    Vector3 temp = new Vector3();
                    for (int t = 0; t < indice.Length; t++)
                    {
                        temp = victim_mesh.vertices[indice[t]];
                        if (temp.x > right_point)
                        {
                            right_point = temp.x;
                        }
                        else if (temp.x < left_point)
                        {
                            left_point = temp.x;
                        }
                        if (temp.y > up_point)
                        {
                            up_point = temp.y;
                        }
                        else if (temp.y < down_point)
                        {
                            down_point = temp.y;
                        }
                        if (front_point > temp.z)
                        {
                            front_point = temp.z;
                        }
                        else if (back_point < temp.z)
                        {
                            back_point = temp.z;
                        }
                    }
                }
            }

            //中心の座標を獲得
            Center_pos.x = (front_point + back_point) / 2.0f;
            Center_pos.y = (up_point + down_point) / 2.0f;
            Center_pos.z = (right_point + left_point) / 2.0f;
            //Debug.Log("center_depth = " + center_depth);
            //Debug.Log("center_length = " + center_length);
            //Debug.Log("center_side = " + center_side);

            // サブメッシュの数だけループ
            for (int sub = 0; sub < victim_mesh.subMeshCount; sub++)
            {
                // サブメッシュのインデックス数を取得
                indices = victim_mesh.GetIndices(sub);

                // サブメッシュのインデックス数分ループ
                for (int i = 0; i < indices.Length; i += 3)
                {
                    // p1 - p3のインデックスを取得。つまりトライアングル
                    indice[0] = indices[i + 0];
                    //Debug.Log("indice[0] = " + indice[0]);
                    indice[1] = indices[i + 1];
                    //Debug.Log("indice[1] = " + indice[1]);
                    indice[2] = indices[i + 2];
                    //Debug.Log("indice[2] = " + indice[2]);



                    //インデックスの追加
                    for (int add_sub = 0; add_sub < victim_child.Length; add_sub++)
                    {
                        victim_child[add_sub].subIndices.Add(new List<int>());
                    }

                    //メッシュがどの場所にあるか計算 左上手前から右で続いて下に行き奥に行きの順
                    for (int t = 0; t < pos.Length; t++)
                    {
                        Vector3 temp = victim_mesh.vertices[indice[t]];
                        if (temp.z < Center_pos.z)
                        {
                            if (temp.y > Center_pos.y)
                            {
                                if (temp.x < Center_pos.x)
                                {
                                    pos[t] = 0;
                                }
                                else
                                {
                                    pos[t] = 1;
                                }
                            }
                            else
                            {
                                if (temp.x < Center_pos.x)
                                {
                                    pos[t] = 2;
                                }
                                else
                                {
                                    pos[t] = 3;
                                }
                            }
                        }
                        else
                        {
                            if (temp.y > Center_pos.y)
                            {
                                if (temp.x < Center_pos.x)
                                {
                                    pos[t] = 4;
                                }
                                else
                                {
                                    pos[t] = 5;
                                }
                            }
                            else
                            {
                                if (temp.x < Center_pos.x)
                                {
                                    pos[t] = 6;
                                }
                                else
                                {
                                    pos[t] = 7;
                                }
                            }
                        }
                    }

                    if(pos[0] == pos[1] && pos[0] == pos[2])
                    {
                        victim_child[pos[0]].AddTriangle(indice, sub);
                    }
                    else
                    {
                        if (pos[0] == pos[1] || pos[0] == pos[2] || pos[1] == pos[2])
                        {

                        }
                        else
                        {
                            Cut_this_Face(sub, indice[0], indice[1], indice[2], pos);
                        }
                    }

                    //Debug.Log("レミリア・スカーレット");
                    //メッシュが中間地点にある場合はカット
                    //Debug.Log("パチュリー・ノーレッジ");
                }
            }
            //Fill_hole();

            Material[] mats = victim.GetComponent<MeshRenderer>().sharedMaterials;

            //メッシュの制作
            Mesh[] one_eight_mesh = new Mesh[8];
            for (int i = 0; i < one_eight_mesh.Length; i++)
            {
                one_eight_mesh[i] = new Mesh();
                one_eight_mesh[i].name = "No_" + (i + 1) + "cube";
                one_eight_mesh[i].SetVertices(victim_child[i].vertices);
                one_eight_mesh[i].triangles = victim_child[i].triangles.ToArray();
                one_eight_mesh[i].normals = victim_child[i].normals.ToArray();
                one_eight_mesh[i].uv = victim_child[i].uvs.ToArray();
                one_eight_mesh[i].subMeshCount = victim_child[i].subIndices.Count;
                for (int t = 0; t < victim_child[i].subIndices.Count; t++)
                {
                    one_eight_mesh[i].SetIndices(victim_child[i].subIndices[t].ToArray(), MeshTopology.Triangles, t);
                    //for(int debug_text = 0; debug_text < victim_child[i].subIndices[t].Count; debug_text++)
                    //{
                    //    Debug.Log("ﾊﾅｶﾞｻｲﾀ[" + t + "]:" + victim_child[i].subIndices[t][debug_text]);
                    //    //Debug.Log()

                    //}
                }
                //Debug.Log(victim_child[i].subIndices.Count);
                //Debug.Log("495年の波紋");

            }

            //最初のゲームオブジェクトだけ設定
            victim.name = "No_1cube";
            Vector3 temp_velocity = victim.GetComponent<Rigidbody>().velocity;
            victim.GetComponent<MeshFilter>().mesh = one_eight_mesh[0];

            GameObject[] cube_obj = new GameObject[8];
            cube_obj[0] = victim;
            //Object.Destroy(cube_obj[0].GetComponent<BoxCollider>());
            //_ = cube_obj[0].AddComponent(typeof(BoxCollider));
            //cube_obj[0].AddComponent(typeof(Rigidbody));
            //Object.Destroy(cube_obj[0], 7.0f);
            //Object.Destroy(cube_obj[0].GetComponent<Space_Cut_Cube>());
            //Rigidbody temp_rigid = cube_obj[0].GetComponent<Rigidbody>();
            //temp_rigid.AddForce(temp_velocity / 8);
            //分割回数を入
            Verosity_Cut_Cube temp_cut = cube_obj[0].GetComponent<Verosity_Cut_Cube>();
            int cutcount = temp_cut.Cut_Count + 1;
            //Debug.Log("cutcount = " + cutcount);
            //int cutcount = temp_cut.cut_count + 1;
            temp_cut.Cut_Count = cutcount;

            //temp_cut.Cut_Count = cutcount;


            //ゲームオブジェクトにメッシュを適応
            for (int i = 1; i < one_eight_mesh.Length; i++)
            {
                cube_obj[i] = new GameObject("No_" + (i + 1) + "cube", typeof(MeshFilter), typeof(MeshRenderer));
                cube_obj[i].transform.position = victim.transform.position;
                cube_obj[i].transform.rotation = victim.transform.rotation;
                cube_obj[i].transform.localScale = victim.transform.localScale;
                cube_obj[i].GetComponent<MeshFilter>().mesh = one_eight_mesh[i];
                cube_obj[i].GetComponent<MeshRenderer>().materials = mats;
                //cube_obj[i].AddComponent(typeof(BoxCollider));
                cube_obj[i].AddComponent(typeof(Rigidbody));
                //temp_rigid = cube_obj[0].GetComponent<Rigidbody>();
                //temp_rigid.AddForce(temp_velocity/8);
                cube_obj[i].AddComponent(typeof(Verosity_Cut_Cube));

                temp_cut = cube_obj[i].GetComponent<Verosity_Cut_Cube>();
                temp_cut.Cut_Count = cutcount;
                //Object.Destroy(cube_obj[i], 5.0f);
            }

            return cube_obj;
        }


        /// <summary>
        /// 全ての辺をカット　1トライアングル制作
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="index1">頂点１</param>
        /// <param name="index2">頂点２</param>
        /// <param name="index3">頂点３</param>
        private static void Cut_this_Face(int sub, int index1, int index2, int index3, int[] pos)
        {
            //中央の三角のポイント
            int center_point;
            //それぞれの辺の長さ
            float[] lengths = new float[3];

            //for(int i = 0; i < pos.Length; i++)
            //{
            //    Debug.Log("pos[" + i + "]:" + pos[i]);
            //}

            //それぞれの情報保持
            Vector3[] points = new Vector3[3];
            Vector2[] uvs = new Vector2[3];
            Vector3[] normals = new Vector3[3];

            //ポイント保存
            points[0] = victim_mesh.vertices[index1];
            points[1] = victim_mesh.vertices[index2];
            points[2] = victim_mesh.vertices[index3];

            //uv保存
            uvs[0] = victim_mesh.uv[index1];
            uvs[1] = victim_mesh.uv[index2];
            uvs[2] = victim_mesh.uv[index3];

            //法線保存
            normals[0] = victim_mesh.normals[index1];
            normals[1] = victim_mesh.normals[index2];
            normals[2] = victim_mesh.normals[index3];

            //for(int i = 0; i < normals.Length; i++)
            //{
            //    Debug.Log("normal[" + i + "]: = " + normals[i]);
            //}

            //長さ保存
            lengths[0] = (points[0] - points[1]).magnitude;
            lengths[1] = (points[0] - points[2]).magnitude;
            lengths[2] = (points[1] - points[2]).magnitude;

            //Debug.Log("クランベリー・トラップ");

            //中央のポイント探索
            if (lengths[0] > lengths[1] && lengths[0] > lengths[2])
            {
                center_point = index3;
            }
            else if (lengths[1] > lengths[0] && lengths[1] > lengths[2])
            {
                center_point = index2;
            }
            else if (lengths[2] > lengths[0] && lengths[2] > lengths[1])
            {
                center_point = index1;
            }
            else
            {
                Debug.Log("error: same length");
                center_point = index1;
                //Object.Destroy(victim);
                //break;
            }


            //三つの中間の頂点を作成
            Vector3 new_vector1 = Vector3.Lerp(points[0], points[1], 0.5f);
            Vector2 new_uv1 = Vector2.Lerp(uvs[0], uvs[1], 0.5f);
            Vector3 new_normal1 = Vector3.Lerp(normals[0], normals[1], 0.5f);

            Vector3 new_vector2 = Vector3.Lerp(points[0], points[2], 0.5f);
            Vector2 new_uv2 = Vector2.Lerp(uvs[0], uvs[2], 0.5f);
            Vector3 new_normal2 = Vector3.Lerp(normals[0], normals[2], 0.5f);

            Vector3 new_vector3 = Vector3.Lerp(points[1], points[2], 0.5f);
            Vector2 new_uv3 = Vector2.Lerp(uvs[1], uvs[2], 0.5f);
            Vector3 new_normal3 = Vector3.Lerp(normals[2], normals[1], 0.5f);
            //Debug.Log("new_normal1 = " + new_normal1);
            //Debug.Log("new_normal2 = " + new_normal2);
            //Debug.Log("new_normal3 = " + new_normal3);

            //Debug.Log("恋の迷路");

            //new_normal1 = new Vector3(0, 1, 0);

            //if (pos[0] == 0 || pos[0] == 1 || pos[0] == 2 || pos[0] == 3)
            //{
            //    if(pos[1] == 0 || pos[1] == 1 || pos[1] == 2 || pos[1] == 3)
            //    {
            //        if(pos[1] == 0 || pos[1] == 1 || pos[1] == 2 || pos[1] == 3)
            //        {

            //        }
            //    }
            //}



            //四つの三角を作成 作った頂点も保存
            if (center_point == index1)
            {
                victim_child[pos[0]].AddTriangle(
                    new Vector3[] { new_vector2, points[0], new_vector1 },
                    new Vector3[] { new_normal3, new_normal3, new_normal3 },
                    new Vector2[] { new_uv2, uvs[0], new_uv1 },
                    new_normal3,
                    sub
                );
                victim_child[pos[0]].AddTriangle(
                    new Vector3[] { new_vector2, new_vector1, new_vector3 },
                    new Vector3[] { new_normal3, new_normal3, new_normal3 },
                    new Vector2[] { new_uv2, new_uv1, new_uv3 },
                    new_normal3,
                    sub
                );
                victim_child[pos[1]].AddTriangle(
                    new Vector3[] { new_vector3, new_vector1, points[1] },
                    new Vector3[] { new_normal3, new_normal3, new_normal3 },
                    new Vector2[] { new_uv3, new_uv1, uvs[1] },
                    new_normal3,
                    sub
                );
                victim_child[pos[2]].AddTriangle(
                    new Vector3[] { points[2], new_vector2, new_vector3 },
                    new Vector3[] { new_normal3, new_normal3, new_normal3 },
                    new Vector2[] { uvs[2], new_uv2, new_uv3 },
                    new_normal3,
                    sub
                );

                victim_child[pos[0]].new_across_vertices.Add(new_vector3);
                victim_child[pos[1]].new_across_vertices.Add(new_vector3);
                victim_child[pos[2]].new_across_vertices.Add(new_vector3);

                victim_child[pos[0]].new_half_vertices.Add(new_vector1);
                victim_child[pos[1]].new_half_vertices.Add(new_vector1);

                victim_child[pos[0]].new_half_vertices.Add(new_vector2);
                victim_child[pos[2]].new_half_vertices.Add(new_vector2);

            }
            else if (center_point == index2)
            {
                victim_child[pos[1]].AddTriangle(
                    new Vector3[] { new_vector1, points[1], new_vector3 },
                    new Vector3[] { new_normal2, new_normal2, new_normal2 },
                    new Vector2[] { new_uv1, uvs[1], new_uv3 },
                    new_normal2,
                    sub
                );
                victim_child[pos[1]].AddTriangle(
                    new Vector3[] { new_vector1, new_vector3, new_vector2 },
                    new Vector3[] { new_normal2, new_normal2, new_normal2 },
                    new Vector2[] { new_uv1, new_uv3, new_uv2 },
                    new_normal2,
                    sub
                );
                victim_child[pos[2]].AddTriangle(
                    new Vector3[] { new_vector2, new_vector3, points[2] },
                    new Vector3[] { new_normal2, new_normal2, new_normal2 },
                    new Vector2[] { new_uv2, new_uv3, uvs[2] },
                    new_normal2,
                    sub
                );
                victim_child[pos[0]].AddTriangle(
                    new Vector3[] { points[0], new_vector1, new_vector2 },
                    new Vector3[] { new_normal2, new_normal2, new_normal2 },
                    new Vector2[] { uvs[0], new_uv1, new_uv2 },
                    new_normal2,
                    sub
                );

                //if (!victim_child[pos[1]].new_across_vertices.Contains(new_vector2))
                //{
                victim_child[pos[1]].new_across_vertices.Add(new_vector2);
                victim_child[pos[0]].new_across_vertices.Add(new_vector2);
                victim_child[pos[2]].new_across_vertices.Add(new_vector2);
                //}

                victim_child[pos[1]].new_half_vertices.Add(new_vector1);
                victim_child[pos[0]].new_half_vertices.Add(new_vector1);

                victim_child[pos[1]].new_half_vertices.Add(new_vector3);
                victim_child[pos[2]].new_half_vertices.Add(new_vector3);
            }
            else if (center_point == index3)
            {
                victim_child[pos[2]].AddTriangle(
                    new Vector3[] { new_vector3, points[2], new_vector2 },
                    new Vector3[] { new_normal1, new_normal1, new_normal1 },
                    new Vector2[] { new_uv3, uvs[2], new_uv2 },
                    new_normal1,
                    sub
                );
                victim_child[pos[2]].AddTriangle(
                    new Vector3[] { new_vector3, new_vector2, new_vector1 },
                    new Vector3[] { new_normal1, new_normal1, new_normal1 },
                    new Vector2[] { new_uv3, new_uv2, new_uv1 },
                    new_normal1,
                    sub
                );
                victim_child[pos[0]].AddTriangle(
                    new Vector3[] { new_vector1, new_vector2, points[0] },
                    new Vector3[] { new_normal1, new_normal1, new_normal1 },
                    new Vector2[] { new_uv1, new_uv2, uvs[0] },
                    new_normal1,
                    sub
                );
                victim_child[pos[1]].AddTriangle(
                    new Vector3[] { points[1], new_vector3, new_vector1 },
                    new Vector3[] { new_normal1, new_normal1, new_normal1 },
                    new Vector2[] { uvs[1], new_uv3, new_uv1 },
                    new_normal1,
                    sub
                );

                //if (!victim_child[pos[2]].new_across_vertices.Contains(new_vector1))
                //{
                victim_child[pos[2]].new_across_vertices.Add(new_vector1);
                victim_child[pos[0]].new_across_vertices.Add(new_vector1);
                victim_child[pos[1]].new_across_vertices.Add(new_vector1);
                //}

                victim_child[pos[2]].new_half_vertices.Add(new_vector3);
                victim_child[pos[1]].new_half_vertices.Add(new_vector3);

                victim_child[pos[2]].new_half_vertices.Add(new_vector2);
                victim_child[pos[0]].new_half_vertices.Add(new_vector2);
            }
        }

        /// <summary>
        /// 空いた穴を埋める
        /// </summary>
        private static void Fill_hole()
        {
            //xyz面の法線を保存
            Vector3[] hole_normal = new Vector3[3];
            //xyz面の頂点を保存
            Vector3[,] xyz_vertics_set = new Vector3[3, 2];

            Vector2 uvs1 = new Vector2();
            Vector2 uvs2 = new Vector2();

            //hole_normal = new Vector3(1.0f, 0, 0);

            for (int i = 0; i < victim_child.Length; i++)
            {
                //重複した値を消す
                victim_child[i].new_across_vertices = Clean_Up(victim_child[i].new_across_vertices);
                //victim_child[i].new_half_vertices = Clean_Up(victim_child[i].new_half_vertices);

                //for (int tes = 0; tes < victim_child[i].new_across_vertices.Count; tes++)
                //{
                //    Debug.Log("across[" + tes + "] = " + victim_child[i].new_across_vertices[tes]);

                //}
                //Debug.Log("あなたがコンテニューできないのさ！");
                //インデックスの追加
                //for (int add_sub = 0; add_sub < victim_child.Length; add_sub++)
                //{
                //    victim_child[add_sub].subIndices.Add(new List<int>());

                //}

                //法線作成
                Create_normal(victim_child[i].new_across_vertices[0], victim_child[i].new_across_vertices[1], i, ref xyz_vertics_set, ref hole_normal);
                Create_normal(victim_child[i].new_across_vertices[0], victim_child[i].new_across_vertices[2], i, ref xyz_vertics_set, ref hole_normal);
                Create_normal(victim_child[i].new_across_vertices[1], victim_child[i].new_across_vertices[2], i, ref xyz_vertics_set, ref hole_normal);


                //Debug.Log("My Hert Break");

                victim_child[i].AddTriangle(
                    new Vector3[]
                    {
                            xyz_vertics_set[0,0],
                            xyz_vertics_set[0,1],
                            Center_pos
                    },
                    new Vector3[]
                    {
                        hole_normal[0],
                        hole_normal[0],
                        hole_normal[0]
                    },
                    new Vector2[]
                    {
                            uvs1,
                            uvs2,
                            new Vector2(0.5f,0.5f)
                    },
                    hole_normal[0],
                    victim_child[i].subIndices.Count - 1
                    );

                victim_child[i].AddTriangle(
                    new Vector3[]
                    {
                            xyz_vertics_set[1,0],
                            xyz_vertics_set[1,1],
                            Center_pos
                    },
                    new Vector3[]
                    {
                        hole_normal[1],
                        hole_normal[1],
                        hole_normal[1]
                    },
                    new Vector2[]
                    {
                            uvs1,
                            uvs2,
                            new Vector2(0.5f,0.5f)
                    },
                    hole_normal[1],
                    victim_child[i].subIndices.Count - 1
                    );

                victim_child[i].AddTriangle(
                    new Vector3[]
                    {
                            xyz_vertics_set[2,0],
                            xyz_vertics_set[2,1],
                            Center_pos
                    },
                    new Vector3[]
                    {
                        hole_normal[2],
                        hole_normal[2],
                        hole_normal[2]
                    },
                    new Vector2[]
                    {
                            uvs1,
                            uvs2,
                            new Vector2(0.5f,0.5f)
                    },
                    hole_normal[2],
                    victim_child[i].subIndices.Count - 1
                    );

                for (int t = 0; t < victim_child[i].new_half_vertices.Count; t++)
                {
                    Vector3 vector_temp = new Vector3();
                    vector_temp = victim_child[i].new_half_vertices[t];
                    if (System.Math.Abs(vector_temp.x - xyz_vertics_set[0, 0].x) < float.Epsilon && System.Math.Abs(vector_temp.x - xyz_vertics_set[0, 1].x) < float.Epsilon)
                    {
                        victim_child[i].AddTriangle(
                            new Vector3[]
                            {
                                vector_temp,
                                xyz_vertics_set[0,0],
                                xyz_vertics_set[0,1]

                            },
                            new Vector3[]
                            {
                                hole_normal[0],
                                hole_normal[0],
                                hole_normal[0]
                            },
                            new Vector2[]
                            {
                                    uvs1,
                                    uvs2,
                                    new Vector2(0.5f,0.5f)
                            },
                            hole_normal[0],
                            victim_child[i].subIndices.Count - 1
                            );
                    }
                    else if (System.Math.Abs(vector_temp.y - xyz_vertics_set[1, 0].y) < float.Epsilon && System.Math.Abs(vector_temp.y - xyz_vertics_set[1, 1].y) < float.Epsilon)
                    {
                        victim_child[i].AddTriangle(
                            new Vector3[]
                            {
                                vector_temp,
                                xyz_vertics_set[1,0],
                                xyz_vertics_set[1,1]

                            },
                            new Vector3[]
                            {
                                hole_normal[1],
                                hole_normal[1],
                                hole_normal[1]
                            },
                            new Vector2[]
                            {
                                    uvs1,
                                    uvs2,
                                    new Vector2(0.5f,0.5f)
                            },
                            hole_normal[1],
                            victim_child[i].subIndices.Count - 1
                            );
                    }
                    else if (System.Math.Abs(vector_temp.z - xyz_vertics_set[2, 0].z) < float.Epsilon && System.Math.Abs(vector_temp.z - xyz_vertics_set[2, 1].z) < float.Epsilon)
                    {
                        victim_child[i].AddTriangle(
                            new Vector3[]
                            {
                                vector_temp,
                                xyz_vertics_set[2,0],
                                xyz_vertics_set[2,1]

                            },
                            new Vector3[]
                            {
                                hole_normal[2],
                                hole_normal[2],
                                hole_normal[2]
                            },
                            new Vector2[]
                            {
                                    uvs1,
                                    uvs2,
                                    new Vector2(0.5f,0.5f)
                            },
                            hole_normal[2],
                            victim_child[i].subIndices.Count - 1
                            );
                    }
                }
            }
        }




        /// <summary>
        /// 法線の向きを探索 参照渡しした値でxyzの面どの方向になるかわかる 1がx面　2がy面 3がz面
        /// </summary>
        /// <param name="point1">頂点</param>
        /// <param name="point2">頂点</param>
        /// <param name="victim_num">被害者ナンバー</param>
        /// <param name="xyz_vertics">面の頂点を保存</param>
        /// <param name="xyz_normal">面の法線保存</param>
        /// <returns></returns>
        private static void Create_normal(Vector3 point1, Vector3 point2, int victim_num, ref Vector3[,] xyz_vertics, ref Vector3[] xyz_normal)
        {
            Vector3 hole_normal = new Vector3();

            //xラインの法線
            if (System.Math.Abs(point1.x - Center_pos.x) < float.Epsilon && System.Math.Abs(point2.x - Center_pos.x) < float.Epsilon)
            {
                if (victim_num % 2 == 0)
                {
                    hole_normal = new Vector3(1.0f, 0, 0);
                }
                else
                {
                    hole_normal = new Vector3(-1.0f, 0, 0);
                }
                xyz_vertics[0, 0] = point1;
                xyz_vertics[0, 1] = point2;
                xyz_normal[0] = hole_normal;
                //return hole_normal;
            }
            //yラインの法線
            else if (System.Math.Abs(point1.y - Center_pos.y) < float.Epsilon && System.Math.Abs(point2.y - Center_pos.y) < float.Epsilon)
            {
                if (victim_num == 0 || victim_num == 1 || victim_num == 4 || victim_num == 5)
                {
                    hole_normal = new Vector3(0, -1.0f, 0);
                }
                else
                {
                    //Debug.Log("スカーレット・シュート！");
                    hole_normal = new Vector3(0, 1.0f, 0);
                }
                xyz_vertics[1, 0] = point1;
                xyz_vertics[1, 1] = point2;
                xyz_normal[1] = hole_normal;
                //return hole_normal;
            }
            //zラインの法線
            else if (System.Math.Abs(point1.z - Center_pos.z) < float.Epsilon && System.Math.Abs(point2.z - Center_pos.z) < float.Epsilon)
            {
                //Debug.Log("スカーレット・シュート！");

                if (victim_num < 4)
                {
                    hole_normal = new Vector3(0, 0, 1.0f);
                    //Debug.Log("スカーレット・シュート！");

                }
                else
                {
                    hole_normal = new Vector3(0, 0, -1.0f);
                }
                xyz_vertics[2, 0] = point1;
                xyz_vertics[2, 1] = point2;
                xyz_normal[2] = hole_normal;
                //return hole_normal;
            }
            //Debug.Log("point1:" + point1 + " point2:" + point2);
            //return hole_normal　* (-1);
        }


        //重複した数値を消す
        private static List<Vector3> Clean_Up(List<Vector3> temp)
        {
            List<Vector3> temp_vec3 = new List<Vector3>();
            foreach (Vector3 pos in temp)
            {
                if (!temp_vec3.Contains(pos))
                {
                    temp_vec3.Add(pos);
                }
            }
            return temp_vec3;
        }
    }

}
