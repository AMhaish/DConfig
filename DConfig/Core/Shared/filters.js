angular.module('DConfigSharedLib')
    .filter('range', function () {
        return function (input, current, total, minValue, maxValue) {
            var middle = parseInt(total / 2);
            var start;
            var end;
            if (current - minValue < middle) {
                start = minValue;
                end = minValue + total - 1;
                if (maxValue - current < middle + 1) {
                    end = maxValue;
                }
            }
            else if (maxValue - current < middle + 1) {
                end = maxValue;
                start = maxValue - total + 1;
            }
            else {
                start = current - middle;
                end = current + middle;
            }
            for (var i = start; i <= end; i++)
                input.push(i);
            if (input.length == 1)
            {
                input = [];
            }
            return input;
        };
    });