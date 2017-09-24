

GameThingInfo



/*************
* OVERRIDES **
*************/

/*
Overrides are your entry point for customizing the behavior of your game things.

You only have to use the overrides that you need.

For example, if you wanted to make a game thing
that turns red when the R key is pressed, you would:

--Make a sub-class of game thing
--Override its keyDown function

The code would look like this:


public class ThingThatTurnsRed : GameThing
{
	


	protected override void keyDown(KeyCode keyCode) {
	
		// if keyCode == KeyCode.R

		// then..

		// turn red
	}



}


*/

protected override void awake() {

	//Put code that should happen when 
	//the thing comes into existence here.
	//
	//The thing might come into existence at the beginning of the game
	//or later, if, for example, the thing 
	//is a clone
}



protected override void start() { 

	//Code that you put here will happen
	//after any code that you put in 'awake()'
	//
	//If your game thing needs to interact with other
	//parts of your game, put the code that accomplishes
	//this here, instead of in awake.  If you try to talk to 
	//another component in your game in awake(), if might be
	//awake yet (so to speak).
}




protected override void LateStart() { 

	//Code here will execute a few seconds after normal start

}


protected override void update() { 

	// put code that should execute once every frame here
	// but for movement related code, using 'fixedUpdate' is better
}


protected override void fixedUpdate() { 

	// code here will execute every 'physics frame'
	// putting code that moves or boosts your game thing here is a good idea.
}



protected override void collisionEnterWithSomethingTagged(TaggedCollision tag) { 

	// Put code here to react to being hit by or running into something

}



protected override void mouseDrag(VectorXY global) { 

	//Similar story for mouseDrag...
}


protected override void mouseUp(VectorXY global) { 

	//and mouseUp
}



protected override void keyDown(KeyCode kc) {
	
	//Override for key presses.
	//Remember to add the keys that you need in 'other keys' in the inspector    
}



protected override void mouseDown(VectorXY global) { 

	//Do something when the left mouse button goes down,
	//the 'global' variable will be the mouse's position in world space
}




/**************
* FUNCTIONS ***
**************/

/*
 * These are game thing functions that you can
 * use, but can't override.
 */

// EXAMPLE: play disco sound effects when the player presses 'S'

	protected override void keyDown(KeyCode keyCode) {
	
		if (keyCode == KeyCode.S){

			play("funky-disco"); 

		}

	}

// use this to turn physics on or off for your game thing 
void physicsWorksOnMe(bool yesItDoes);



//To say something (text appears near the game thing)
void say(string words);



//To announce something (text appears near the top of the screen)
void announce(string words);


 // play a sound. The audio file for the sound (for example an mp3 file) has to be inside of Assets/Resources/Audio/Clip 
void play(string clipPathResourcesAudioClipRelative);


// make a copy of the game thing
void clone();


// make a copy of the game thing and place it somewhere
void clone(Vector3 global);


// add a particle system
void addParticles(string particlePrefabName);


// play the particle system (make is start producing bubbles, fire, etc.)
void playParticles(string particleName);


// stop playing the particle system
void stopParticles(string particleName);


// destroy the particle system (if you don't need it anymore and want to save memory)
void destroyParticles(string particleName);



// Change the background. (the argument needs to be the name of an image in the backgrounds folder.) 
//
//  EXAMPLE:  setBackground("sunset.jpg"); 
//
void setBackground(string backgroundInBackgroundsFolderName);



//Set the background color (will tint the background image)
void setBackgroundColor(Color c);


// Move forward (meaning right or in the direction of the red arrow) 
// if the gamething is rotated this won't be the same as global right 
void moveForward();


// Move in any direction 
void moveInDirection(Vector3 dir);


// Teleport somewhere
void moveTo(Vector3 global);



// Go part of the way towards a location
// Useful if you want to gradually slide somewhere
//
// You can call this function in two ways: 
//
//   lerpTo(someLocationXYZ); // lerpFactor defaults to 1.4
// 
//  or
// 
//   lerpTo(someLocationXYZ, 4f);  //the larger the lerp the faster you'll get there
//
void lerpTo(Vector3 global, float lerpFactor = 1.4f);




// Use this to get boosted in a direction
// if the direction is upwards, the game thing will jump
void boost(Vector2 force);



// Turn the game thing by some degrees.
void rotate(float byDegrees);



// Turn the game thing so that it is looking at a location
void lookAt(Vector3 target);




// Gradually turn the game things so that it is looking at a location
// The larger the slerp, the faster the turn.
void slerpLookAt(Vector3 target, float slerp);




// Change color
void changeColor(Color c);





/***************
 * Data ********
 ***************/


/**************
*
Some of these are 'read/write', meaning that you can change them in your code
Others are read-only, meaning you can get info from them but not change them.
*
***************/

/*
 * Example: change speeds by changing the value of 'speed'
 *

protected override void keyDown(KeyCode keyCode) {
		
	if(keyCode == KeyCode.E) {

		speed = 5f;

	} 

	if(keyCode == KeyCode.G) {

		speed = 10f;

	}
}

*/

// how fast is this game thing?
float speed;


// this bool is true for game things that are clones
bool isAClone;


//Are this thing's feet on the ground?
bool isGrounded;


//Time the game has been running in seconds
float time


//A value that bounces between negative one and one over time
float sinWaveTime(float period)


//The game thing's x, y, z postion
Vector3 position


//The game thing's rotation in degrees
float rotationDegrees 



//the cursor or mouse position in world space
protected Vector3 mousePosition



Collider2D colldr;



Rigidbody2D rb;



SpriteRenderer srendrr;





