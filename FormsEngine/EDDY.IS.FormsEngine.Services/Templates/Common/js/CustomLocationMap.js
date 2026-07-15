//(function ($) {
var map;
var map_div = 'map_canvas';
var map_div_container = "map_canvas_container";


	jQuery(document).ready(function(){

	    jQuery('#' + map_div_container + ' .close').on('click', function (e) {
		    e.preventDefault();
		    jQuery('#' + map_div_container).hide();

		});
		 

	});
	//End Document Ready
	
	function initializeMap(map_div) {			

		if(jQuery('#' + map_div).length >0){
		    
			map = new google.maps.Map(document.getElementById(map_div), {
				//center: new google.maps.LatLng(0,0),
				zoom: 3,
				mapTypeId: google.maps.MapTypeId.ROADMAP,
				panControl: true,
				streetViewControl: true,
				mapTypeControl: true
			});

			
		}

	}		
		
	
	function view_map(campus_address, campus_name, e) {

        if(map == null || map === undefined)
            initializeMap(map_div);
		    var geocoder = new google.maps.Geocoder();	 
			//pass address striped html
			geocoder.geocode( { 'address': campus_address.replace(/<\/?[^>]+(>|$)/g, "")}, function(results, status) {
				if (status == google.maps.GeocoderStatus.OK) {
					createMarker(campus_address, results[0].geometry.location)

				}else{
				
				}
			});

			jQuery('#' + map_div_container).show();

			if (jQuery(e.target).closest('.mc-US-Display'))
			    jQuery('#' + map_div_container).css('top', e.pageY - jQuery('#mc-US-Area').offset().top - 80 + 'px');

			if (jQuery(e.target).parents().hasClass('mc-SM-Display'))
			    jQuery('#' + map_div_container).css('top', e.pageY - jQuery('#mc-SM-Area').offset().top - 60 + 'px');

			jQuery('#' + map_div_container).find('[name="campus-school-name"]').html(campus_name);

	}

// ======= Function to create a marker
function createMarker(address, latlng) {

   var contentString = address;
   var marker = new google.maps.Marker({
	 position: latlng,
	 map: map
	 //zIndex: Math.round(latlng.lat()*-100000)<<5
   });
   
	
	map.setCenter(latlng);
	map.setZoom(12);

	
}
