# secondMaking
個人制作用のリポジトリ

360度全方位シューティングゲームで
MPだけに一本化したシステムで、準備、戦闘の二つのフェーズを入れるような、ストラテジー要素を取り入れる

**2019/9/1**

プレイヤーの移動をテストシーン上で実装
------------------------------------

TPSカメラは事前に作っていたプログラムを改良して組み込み。
    
プレイヤーは3次元の立体的な移動が必要なため、地上での移動と空中での移動を状態管理で差別化を図った。
実装時では、プレイヤーの動きをEnumでGroundステートとHoverステートを定義し、ステートに応じた移動を実装した。
 
**各ステートでの動きの概要**
    
- Groundステート
 
 重力を適用したうえで、前後左右とジャンプの基本的な動きを実装。ジャンプなどで空中にいる場合、一定時間内にジャンプするキーでHover状態に移行

- Hoverステート
    
 重力を適用させないようにし、前後左右の動きに上昇を加え、CtrlキーでGroundステートに移行して重力を適用して下降を可能に。

スプリント時は、両ステート共通でキャラクターを移動方向に向かせる。通常時は常にカメラの向きを向くように実装。

**2019/9/10**

カメラのFOV値の変化とIKによる銃の持ち手の調整
-------------------------------------------

右クリックで銃ののぞき込み動作をするためFOV値を絞り、スプリント時には疾走感を増すためにFOV値をを広げるようにする。  
カメラの制御は壁抜けをさせないためCinemacineのVirtualCameraを使用した。

キャラクターが画面の正面に向かって銃を向けるように、カメラの正面からRayを発射し、  
当たった位置に銃を向けさせ、向けた銃に対して両手の位置をIKで指定する形をとった。

スプリント時は銃をカメラ方向に向けさせずに、銃を持つ手をIKで位置指定する処理のみ行う。
