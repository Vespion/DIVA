namespace VespionSoftworks.DIVA.Discordia.Models;

public interface IEntity<out TId>
	where TId : IEquatable<TId>
{
	/// <summary>
	///     Gets the unique identifier for this object.
	/// </summary>
	TId Id { get; }

}

public interface IHaveSnowflakeId: IEntity<SnowflakeId>;