public interface IMovableCharacter {
  void Move(float speedScalar);
  void Dash(bool dashing);
  void Jump(bool jumping);
}