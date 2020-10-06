var measure_container = document.getElementById("measure_container");
var measure_firstInit = false;
var measure_d;
var measure_count = 2;
var measure_pos = new Array(measure_count);
function InitMeasure() {
    measure_d = direction.GetWindowLength() * 0.8;
    let start = direction.GetWindowLength() * 0.1;
    measure_pos[0] = start;
    measure_pos[1] = start + measure_d;
    //measure_pos[2] = start + measure_d * 2;
    direction.SetParallelPositivePos(measure_container.children[0], measure_pos[0]);
    direction.SetParallelPositivePos(measure_container.children[1], measure_pos[1]);
    //direction.SetParallelPositivePos(measure_container.children[2], measure_pos[2]);
}
function MeasureScroll(px) {
    for (let i = 0; i < measure_count; i++) {
        measure_pos[i] = measure_pos[i] + px;
        if (measure_pos[i] < -20) { measure_pos[i] += measure_count * measure_d; }
        else if (measure_pos[i] > direction.GetWindowLength() + 20) {
            measure_pos[i] -= measure_count * measure_d;
        }
    }
    direction.SetParallelPositivePos(measure_container.children[0], measure_pos[0]);
    direction.SetParallelPositivePos(measure_container.children[1], measure_pos[1]);
    //direction.SetParallelPositivePos(measure_container.children[2], measure_pos[2]);
}