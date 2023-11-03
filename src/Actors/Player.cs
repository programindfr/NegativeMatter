using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// Variables publiques
	// Vitesse du joueur
	[Export]
	public int Speed { get; set; } = 400;
	// Valeur du bonnus d'endurance
	[Export]
	public double StaminaSpeedMultiplierValue { get; set; } = 1.15D;
	// Valeur du temps d'endurance
	[Export]
	public double StaminaTimeValue { get; set; } = 3.0D;
	
	
	// Variables internes
	// Bonnus d'endurance
	private double _staminaSpeedMultiplier = 1;
	
	
	// Fonctions internes
	private bool _start_stamina_consumption()
	{
		var timer = GetNode<Timer>("StaminaConsumptionTimer");
		if (timer.Paused)
		{
			timer.Paused = false;
			_stop_stamina_cooldown();
			_stop_stamina_regeneration();
			return true;
		}
		return false;
	}
	
	private void _stop_stamina_consumption()
	{
		GetNode<Timer>("StaminaConsumptionTimer").Paused = true;
		_start_stamina_cooldown();
	}
	
	private void _start_stamina_cooldown()
	{
		GetNode<Timer>("StaminaCooldownTimer").Start();
	}
	
	private void _stop_stamina_cooldown()
	{
		GetNode<Timer>("StaminaCooldownTimer").Stop();
	}
	
	private void _start_stamina_regeneration(double timeLeft)
	{
		GetNode<Timer>("StaminaRegenerationTimer").Start(timeLeft);
	}
	
	private void _stop_stamina_regeneration()
	{
		GetNode<Timer>("StaminaRegenerationTimer").Stop();
	}
	
	
	// _Ready function
	public override void _Ready()
	{
		var timer = GetNode<Timer>("StaminaConsumptionTimer");
		timer.Paused = true;
		timer.Start();
	}
	
	
	// _Process function
	public override void _Process(double delta)
	{
		// Vecteur déplassement du joueur
		Vector2 velocity = Vector2.Zero;

		// Controles déplacement du joueur
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
			if (_start_stamina_consumption())
			{
				_staminaSpeedMultiplier = StaminaSpeedMultiplierValue;
			}
		}
		// _staminaSpeedMultiplier != 1
		else if (Math.Abs(_staminaSpeedMultiplier - 1) > 0.001D)
		{
			_staminaSpeedMultiplier = 1;
			_stop_stamina_consumption();
		}

		// Gestion de l'annimation du joueur
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed * (float)_staminaSpeedMultiplier;
			
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
		
		// Gestion de l'endurance
		var regenerationTimer = GetNode<Timer>("StaminaRegenerationTimer");
		var consumptionTimer = GetNode<Timer>("StaminaConsumptionTimer");
		if (!regenerationTimer.IsStopped())
		{
			consumptionTimer.Start(StaminaTimeValue - regenerationTimer.TimeLeft);
		}
		
		var bar = GetNode<ProgressBar>("ProgressBar");
		bar.Value = consumptionTimer.TimeLeft;
		
		//GD.Print(GetNode<Timer>("StaminaConsumptionTimer").TimeLeft);
	}
	
	
	// Signal function's
	private void _on_stamina_consumption_timer_timeout()
	{
		_staminaSpeedMultiplier = 1;
		_start_stamina_cooldown();
	}
	
	private void _on_stamina_cooldown_timer_timeout()
	{
		var timer = GetNode<Timer>("StaminaConsumptionTimer");
		timer.Paused = true;
		if (Math.Abs(timer.TimeLeft) < 0.001D)
		{
			timer.Start(0.001D);
			_start_stamina_regeneration(StaminaTimeValue);
		}
		else
		{
			timer.Start(timer.TimeLeft);
			_start_stamina_regeneration(StaminaTimeValue - timer.TimeLeft);
		}
	}
}
