window.printRow = function (data) {
        if (!data) {
                alert("No data passed!");
                return;
        }

        console.log("Received data for printing:", data); // For debugging

        const win = window.open('', '', 'width=800,height=400');
        win.document.write('<html><head><title>Print Row</title>');
        win.document.write('<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css">');
        win.document.write('</head><body>');
        win.document.write('<div class="container mt-4">');
        win.document.write('<h5>Examinee Details</h5>');

        win.document.write('<table class="table table-bordered">');
        win.document.write('<thead><tr>');
        win.document.write('<th>Full Name</th>');
        win.document.write('<th>Program ID</th>');
        win.document.write('<th>Schedule ID</th>');
        win.document.write('<th>Municipality ID</th>');
        win.document.write('</tr></thead>');

        win.document.write('<tbody><tr>');
        win.document.write(`<td>${data.fullName}</td>`);
        win.document.write(`<td>${data.programId}</td>`);
        win.document.write(`<td>${data.scheduleId}</td>`);
        win.document.write(`<td>${data.municipalityId}</td>`);
        win.document.write('</tr></tbody>');

        win.document.write('</table>');
        win.document.write('</div></body></html>');

        win.document.close();
        win.print();
};
