using Godot;

public partial class StaminaConsumptionTimer : Timer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _on_player_start_stamina()
	{
		var timer = GetNode<Timer>("StaminaConsumptionTimer");
//		if (timer.Paused)
//		{
//			timer.Paused = false;
//		}
//		else
//		{
//			timer.Start(3.0);
//		}
		timer.Start();
	}

	private void _on_player_stop_stamina()
	{
		//var timer = GetNode<Timer>("StaminaConsumptionTimer");
//		if (timer.Paused == false)
//		{
//			timer.Paused = true;
//		}
	}
}
