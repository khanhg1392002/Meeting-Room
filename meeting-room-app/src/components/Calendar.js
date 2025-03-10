import React, { useState, useEffect } from 'react';
import axios from 'axios';

function Calendar() {
    const [rooms, setRooms] = useState([]);
    const [error, setError] = useState(null);

    useEffect(() => {
        // Gọi API từ backend
        axios
            .get('https://localhost:44340/api/Rooms') // Sử dụng cổng đúng
            .then((response) => {
                console.log('API Response:', response.data); // Log dữ liệu để debug
                setRooms(response.data);
            })
            .catch((error) => {
                console.error('Error fetching rooms:', error); // Log lỗi chi tiết
                setError(error.message || 'An error occurred while fetching rooms.');
            });
    }, []); // Dependency array rỗng để chạy chỉ một lần khi component mount

    return (
        <div style={{ padding: '20px' }}>
            <h1>Meeting Room Calendar</h1>

            {/* Hiển thị lỗi nếu có */}
            {error && <p style={{ color: 'red' }}>Error: {error}</p>}

           

            {/* Bảng hiển thị tất cả thông tin */}
            {rooms.length > 0 ? (
                <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                    <thead>
                        <tr style={{ backgroundColor: '#f2f2f2' }}>
                            <th style={tableHeaderStyle}>ID</th>
                            <th style={tableHeaderStyle}>Room Name</th>
                            <th style={tableHeaderStyle}>Location</th>
                            <th style={tableHeaderStyle}>Capacity</th>
                            <th style={tableHeaderStyle}>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {rooms.map((room) => (
                            <tr key={room.RoomID} style={{ borderBottom: '1px solid #ddd' }}>
                                <td style={tableCellStyle}>{room.RoomID}</td>
                                <td style={tableCellStyle}>{room.RoomName || 'N/A'}</td>
                                <td style={tableCellStyle}>{room.Location || 'N/A'}</td>
                                <td style={tableCellStyle}>{room.Capacity || 'N/A'}</td>
                                <td style={tableCellStyle}>{room.Status || 'N/A'}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            ) : (
                !error && <p>No rooms available to display.</p>
            )}
        </div>
    );
}

// Styles cho bảng
const tableHeaderStyle = {
    padding: '10px',
    textAlign: 'left',
    borderBottom: '2px solid #ddd',
};

const tableCellStyle = {
    padding: '10px',
    textAlign: 'left',
};

export default Calendar;