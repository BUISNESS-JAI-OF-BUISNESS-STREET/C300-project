  $('#getting-started').countdown('2022/12/01', function(event) {
    $(this).html(event.strftime('%w weeks %d days %H:%M:%S'));
  });
