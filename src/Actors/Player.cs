using Godot;

public partial class Player : CharacterBody2D
{
	// Signals
	// Start using stamina
	[Signal]
	public delegate void StartStaminaEventHandler();
	// Stop using stamina
	[Signal]
	public delegate void StopStaminaEventHandler();
	
	// Variables publiques
	// Vitesse du joueur
	[Export]
	public int Speed { get; set; } = 400;
	// Valeur du bonnus d'endurance
	[Export]
	public float StaminaSpeedMultiplierValue { get; set; } = 1.05F;
	
	// Variables internes
	// Bonnus d'endurance
	private float _staminaSpeedMultiplier;
	
	public override void _Ready()
	{
	}
	
	public override void _Process(double delta)
	{
		// Vecteur déplassement du joueur
		Vector2 velocity = Vector2.Zero;

		if (Input.IsActionPressed("player_right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed("player_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("player_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("player_up"))
		{
			velocity.Y -= 1;
		}
		
		if (Input.IsActionPressed("player_sprint"))
		{
			EmitSignal(SignalName.StartStamina);
			_staminaSpeedMultiplier = StaminaSpeedMultiplierValue;
		}
		else if (_staminaSpeedMultiplier != 1)
		{
			EmitSignal(SignalName.StopStamina);
			_staminaSpeedMultiplier = 1;
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed * _staminaSpeedMultiplier;
			
			if (velocity.X < 0)
			{
				animatedSprite2D.Animation = "left";
			}
			else if (velocity.X > 0)
			{
				animatedSprite2D.Animation = "right";
			}
			else if (velocity.Y < 0)
			{
				animatedSprite2D.Animation = "up";
			}
			else
			{
				animatedSprite2D.Animation = "down";
			}
			
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
			animatedSprite2D.Animation = "down";
		}
		
		MoveAndCollide(velocity * (float)delta);
	}
	
	private void _on_stamina_consumption_timer_timeout()
	{
		_staminaSpeedMultiplier = 1;
	}
}
