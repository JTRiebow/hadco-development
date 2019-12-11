A datetime picker for range between two calendars and time sliders. 
https://github.com/rgkevin/datetimeRangePicker/blob/master/range-picker.js
It required Angular 1.2 specifically, so we couldn't add it via bower. 

I made two changes:

in range-picker.js in lines 83-84, I replaced col-xs-6 with columns small-6 to match foundation's syntax. Here's the original:

'<div class="col-xs-6 text-center"><span class="label label-range-picker">{{data.time.from | rgTime:data.time.hours24}}</span></div>' +
'<div class="col-xs-6 text-center"><span class="label label-range-picker">{{data.time.to | rgTime:data.time.hours24}}</span></div>'


I also made some css changes:

line 4, centered the slider by adding margin: 0px auto;

on line 98, of range-picker.css, I added 
overflow: visible; 
to the class .rg-range-picker-slider .rg-range-picker-slider-labels .rg-range-picker-divider 

line 100 change #5cb85c to #B50A37
line 133 change #f4c37d to light grey #e7e7e7
line 137 change #eea236 to dark red #B50A37
170 change #f0ad4e to #B50A37


