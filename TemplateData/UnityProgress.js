function init() {

}


var percent = 0,
update,
resetColors,
speed = 200,
orange = 30,
yellow = 55,
green = 85,
timer;

resetColors = function() {

  $(".progress__bar")
  .removeClass("progress__bar--green")
  .removeClass("progress__bar--yellow")
  .removeClass("progress__bar--orange")
  .removeClass("progress__bar--blue");
  
  $(".unity_progress")
  .removeClass("progress--complete");
  
};

function update(currentProgress) {
  $(".unity_progress").addClass("progress--active");
  var percent = parseFloat( currentProgress.toFixed(1) ) * 100;

  $(".progress__text").find("em").text( percent + "%" );

  if( percent >= 100 ) {

    percent = 100;
    $(".unity_progress").addClass("progress--complete");
    $(".progress__bar").addClass("progress__bar--blue");
    $(".progress__text").find("em").text( "Complete" );

  } else {

    if( percent >= green ) {
      $(".progress__bar").addClass("progress__bar--green");
    }

    else if( percent >= yellow ) {
      $(".progress__bar").addClass("progress__bar--yellow");
    }

    else if( percent >= orange ) {
      $(".progress__bar").addClass("progress__bar--orange");
    }
  }

  $(".progress__bar").css({ width: percent + "%" });
};

function UnityProgress (dom) {
	this.progress = 0.0;
	this.message = "";
	this.dom = dom;
	var parent = dom.parentNode;

	this.SetProgress = function (progress) { 
		if (this.progress < progress) {
			this.progress = progress;
		}
		if (progress == 1) {
			this.SetMessage("Preparing...");
      $(".unity_progress").css("display","none");
    } 
    this.Update();
  }

  this.SetMessage = function (message) { 
    this.message = message; 
    this.Update();
  }

  this.Clear = function() {
    $(".unity_progress").css("display","none");
  }

  this.Update = function() {
    update(this.progress);
    $(".progress__text").html(this.message);
  }

  this.Update();
}





