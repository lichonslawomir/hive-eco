window.formatUtcDates = function (utcDateArray, format) {
    if (!format) {
        return utcDateArray.map(d => new Date(d).toLocaleString());
    }
    return utcDateArray.map(d => new Date(d).toLocaleString());
}
window.formatUtcDate = function (utcDate, format) {
    if (!format) {
        return new Date(utcDate).toLocaleString();
    }
    return new Date(utcDate).toLocaleString();
}