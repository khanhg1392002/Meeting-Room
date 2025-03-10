import React, { useState } from 'react';
import './MeetingForm.css';

const MeetingForm = () => {
  const [formData, setFormData] = useState({
    organizer: '',
    location: 'Lab 6',
    roomInfo: '8th Floor (Block A), 10 Pax, PC, Projector',
    meetingTitle: '',
    date: '',
    startTime: '',
    endTime: '',
    participants: '',
    mailingList: '',
    purpose: '',
    isConfidential: false,
    sendRequest: true,
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value,
    });
  };

  const handlePurposeChange = (e) => {
    setFormData({
      ...formData,
      purpose: e.target.innerHTML,
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log('Form submitted:', formData);
    // Add your booking logic here
  };

  return (
    <div className="meeting-form-container">
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>ORGANIZER</label>
          <input
            type="text"
            name="organizer"
            value={formData.organizer}
            onChange={handleChange}
            placeholder="Enter organizer name..."
          />
        </div>

        <div className="form-group">
          <label>LOCATION / MEETING ROOM</label>
          <select
            name="location"
            value={formData.location}
            onChange={handleChange}
          >
            <option value="Lab 6">Lab 6</option>
            <option value="Lab 6 - ABA">Lab 6 - ABA</option>
          </select>
        </div>

        <div className="form-group">
          <label>ROOM INFORMATION</label>
          <input
            type="text"
            name="roomInfo"
            value={formData.roomInfo}
            onChange={handleChange}
            readOnly
          />
        </div>

        <div className="form-group">
          <label>MEETING TITLE *</label>
          <input
            type="text"
            name="meetingTitle"
            value={formData.meetingTitle}
            onChange={handleChange}
            placeholder="Enter meeting title..."
            required
          />
        </div>

        <div className="form-group date-time-group">
          <label>DATE/TIME MEETING *</label>
          <div className="date-time-inputs">
            <input
              type="text"
              name="date"
              value={formData.date}
              onChange={handleChange}
              placeholder="dd/mm/yyyy"
              className="date-input"
              required
            />
            <span className="calendar-icon">📅</span>
            <input
              type="text"
              name="startTime"
              value={formData.startTime}
              onChange={handleChange}
              placeholder="--:--"
              className="time-input"
              required
            />
            <span className="clock-icon">🕒</span>
            <input
              type="text"
              name="endTime"
              value={formData.endTime}
              onChange={handleChange}
              placeholder="--:--"
              className="time-input"
              required
            />
            <span className="clock-icon">🕒</span>
          </div>
          <span className="date-time-format">
            (dd/mm/yyyy hh:mm - format: 24h)
          </span>
        </div>

        <div className="form-group">
          <label>PARTICIPANTS</label>
          <input
            type="text"
            name="participants"
            value={formData.participants}
            onChange={handleChange}
            placeholder="Select a participants..."
          />
        </div>

        <div className="form-group">
          <label>MAILING LIST</label>
          <input
            type="text"
            name="mailingList"
            value={formData.mailingList}
            onChange={handleChange}
            placeholder="Select a mailing list..."
          />
        </div>

        <div className="form-group">
          <label>PURPOSE</label>
          <div className="purpose-toolbar">
            <button type="button">B</button>
            <button type="button">I</button>
            <button type="button">U</button>
            <button type="button">📌</button>
            <button type="button">🔢</button>
            <button type="button">📋</button>
            <button type="button">🔗</button>
            <button type="button">💾</button>
            <button type="button">📝</button>
            <button type="button">🖼️</button>
            <button type="button">❓</button>
          </div>
          <div
            contentEditable
            name="purpose"
            onInput={handlePurposeChange}
            className="purpose-editor"
            placeholder="Enter purpose here..."
          ></div>
        </div>

        <div className="form-options">
          <label>
            <input
              type="checkbox"
              name="sendRequest"
              checked={formData.sendRequest}
              onChange={handleChange}
            />{' '}
            Send Meeting Request
          </label>
          <label>
            <input
              type="checkbox"
              name="isConfidential"
              checked={formData.isConfidential}
              onChange={handleChange}
            />{' '}
            Confidential Meeting
          </label>
        </div>

        <div className="form-actions">
          <button type="button" className="calendar-btn">
            Calendar
          </button>
          <button type="submit" className="book-now-btn">
            Book now
          </button>
          <span className="required-note">* Required fields.</span>
        </div>
      </form>
    </div>
  );
};

export default MeetingForm;