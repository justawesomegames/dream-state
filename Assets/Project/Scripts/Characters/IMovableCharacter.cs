public interface IMovableCharacter {
  void Move(float speedScalar);
  void Dash(bool dash);
  void Jump(bool jump);
}