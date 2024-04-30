 // Warning SING01 on `PublicConstructorSingleton`: `The constructor PublicConstructorSingleton.PublicConstructorSingleton() of the singleton class PublicConstructorSingleton has to be private.`
 // Warning CS8618 on `Instance`: `Non-nullable property 'Instance' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.`
 [Singleton]
 internal class PrivateConstructorSingleton
 {
   private PrivateConstructorSingleton()
   {
   }
   public static PrivateConstructorSingleton Instance { get; } = new global::PrivateConstructorSingleton();
 }
 [Singleton]
 internal class PublicConstructorSingleton
 {
   public PublicConstructorSingleton()
   {
   }
   public static PublicConstructorSingleton Instance { get; } = new global::PublicConstructorSingleton();
 }