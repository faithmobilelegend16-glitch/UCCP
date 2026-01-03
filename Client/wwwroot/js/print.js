window.printStockCard = function (data) {
    if (!data || !data.entries) {
        alert("No stock card data provided!");
        return;
    }

    const win = window.open('', '', 'width=1000,height=700');
    win.document.write('<html><head><title>Stock Card</title>');
    win.document.write(`
        <style>
            body {
                font-family: 'Arial', sans-serif;
                font-size: 12px;
                margin: 30px;
            }
            table {
                width: 100%;
                border-collapse: collapse;
            }
            th, td {
                border: 1px solid black;
                padding: 4px;
                text-align: center;
                font-size: 12px;
            }
            .left-align {
                text-align: left;
            }
            .center-title {
                text-align: center;
                font-weight: bold;
                font-size: 16px;
                padding: 8px 0;
            }
            .appendix {
                position: absolute;
                top: 10px;
                right: 40px;
                font-style: italic;
                font-size: 12px;
            }
            .top-info {
                display: flex;
                justify-content: space-between;
                margin-bottom: 4px;
                font-size: 12px;
            }
            .top-info div {
                font-weight: bold;
                margin-right: 51px;
              /*  margin-right: 120px;*/
            }
            .top-info span {
                font-weight: normal;
            }
        </style>
    `);
    win.document.write('</head><body>');

    // Appendix
    win.document.write('<div class="appendix">Appendix 58</div>');

    // Title
    win.document.write('<div class="center-title">STOCK CARD</div>');

    // Outside top info
    win.document.write(`
        <div class="top-info">
            <div>Entity Name: <span>Patin-ay National High School</span></div>
            <div>Fund Cluster: <span>01-Regular</span></div>
        </div>
    `);

    // Start Table
    win.document.write('<table>');

    // Column widths (optional for cleaner alignment)
    win.document.write(`
        <colgroup>
            <col style="width:14%;">
            <col style="width:14%;">
            <col style="width:14%;">
            <col style="width:14%;">
            <col style="width:14%;">
            <col style="width:14%;">
            <col style="width:14%;">
        </colgroup>
    `);

    // Header rows
    win.document.write(`
        <tr>
            <td colspan="5" class="left-align"><strong>Item:</strong> School Card Printing</td>
            <td colspan="3" class="left-align"><strong>Stock No.:</strong></td>
        </tr>
        <tr>
            <td colspan="5" class="left-align"><strong>Description:</strong> (Colored)</td>
            <td colspan="3" class="left-align"><strong>Re-order Point:</strong></td>
        </tr>
        <tr>
            <td colspan="5" class="left-align"><strong>Unit of Measurement:</strong> pieces</td>
            <td colspan="3"></td>
        </tr>
    `);

    // Table headers
    win.document.write(`
        <tr>
            <th rowspan="2">Date</th>
            <th rowspan="2">Reference</th>
            <th>Receipt</th>
            <th colspan="2">Issue</th>
            <th>Balance</th>
            <th rowspan="2">No. of Days to Consume</th>
        </tr>
        <tr>
            <th>Qty.</th>
            <th>Qty.</th>
            <th>Office</th>
            <th>Qty.</th>
        </tr>
    `);

    // Entry rows
    data.entries.forEach(e => {
        win.document.write('<tr>');
        win.document.write(`<td>${e.date || ''}</td>`);
        win.document.write(`<td class="left-align">${e.reference || ''}</td>`);
        win.document.write(`<td>${e.receiptQty || ''}</td>`);
        win.document.write(`<td>${e.issueQty || ''}</td>`);
        win.document.write(`<td class="left-align">${e.office || ''}</td>`);
        win.document.write(`<td>${e.balanceQty || ''}</td>`);
        win.document.write(`<td>${e.daysToConsume || ''}</td>`);
        win.document.write('</tr>');
    });

    // Add blank rows for layout (15 total)
    const totalRows = 15;
    const emptyRows = totalRows - data.entries.length;
    for (let i = 0; i < emptyRows; i++) {
        win.document.write('<tr><td>&nbsp;</td><td></td><td></td><td></td><td></td><td></td><td></td></tr>');
    }

    win.document.write('</table>');
    win.document.write('</body></html>');
    win.document.close();
    win.print();
};







/*
window.printStockCard = function (data) {
    if (!data || !data.entries) {
        alert("No stock card data provided!");
        return;
    }

    const win = window.open('', '', 'width=1000,height=700');
    win.document.write('<html><head><title>Stock Card</title>');
    win.document.write(`
        <style>
            body { font-family: 'Arial', sans-serif; font-size: 12px; }
            .header-table, .data-table { width: 100%; border-collapse: collapse; margin-bottom: 10px; }
            .header-table td { border: 1px solid black; padding: 4px; }
            .data-table th, .data-table td { border: 1px solid black; padding: 4px; text-align: center; }
            .data-table th { font-weight: bold; }
            .center-title { text-align: center; font-weight: bold; margin: 10px 0; font-size: 16px; }
            .appendix { position: absolute; top: 10px; right: 20px; font-style: italic; font-size: 12px; }
        </style>
    `);
    win.document.write('</head><body>');

    // Appendix number
    win.document.write('<div class="appendix">Appendix 58</div>');

    // Title
    win.document.write('<div class="center-title">STOCK CARD</div>');

    // Header info
    win.document.write('<table class="header-table">');
    win.document.write(`
        <tr>
            <td colspan="2">Entity Name: <strong>Patin-ay National High School</strong></td>
            <td colspan="2">Fund Cluster: 01-Regular</td>
        </tr>
        <tr>
            <td colspan="2">Item: School Card Printing</td>
            <td colspan="2">Stock No.:</td>
        </tr>
        <tr>
            <td colspan="2">Description: (Colored)</td>
            <td colspan="2">Re-order Point:</td>
        </tr>
        <tr>
            <td colspan="2">Unit of Measurement: pieces</td>
            <td colspan="2"></td>
        </tr>
    `);
    win.document.write('</table>');

    // Data table with double row headers
    win.document.write('<table class="data-table">');
    win.document.write(`
        <thead>
            <tr>
                <th rowspan="2">Date</th>
                <th rowspan="2">Reference</th>
                <th colspan="1">Receipt</th>
                <th colspan="2">Issue</th>
                <th colspan="1">Balance</th>
                <th rowspan="2">No. of Days to Consume</th>
            </tr>
            <tr>
                <th>Qty.</th>
                <th>Qty.</th>
                <th>Office</th>
                <th>Qty.</th>
            </tr>
        </thead>
        <tbody>
    `);

    // Fill entries
    data.entries.forEach(e => {
        win.document.write('<tr>');
        win.document.write(`<td>${e.date || ''}</td>`);
        win.document.write(`<td>${e.reference || ''}</td>`);
        win.document.write(`<td>${e.receiptQty || ''}</td>`);
        win.document.write(`<td>${e.issueQty || ''}</td>`);
        win.document.write(`<td>${e.office || ''}</td>`);
        win.document.write(`<td>${e.balanceQty || ''}</td>`);
        win.document.write(`<td>${e.daysToConsume || ''}</td>`);
        win.document.write('</tr>');
    });

    // Add blank rows to reach ~15 total for visual layout
    const blankRows = 15 - data.entries.length;
    for (let i = 0; i < blankRows; i++) {
        win.document.write('<tr><td>&nbsp;</td><td></td><td></td><td></td><td></td><td></td><td></td></tr>');
    }

    win.document.write('</tbody></table>');
    win.document.write('</body></html>');
    win.document.close();
    win.print();
};
*/
