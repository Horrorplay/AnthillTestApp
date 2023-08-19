import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';

const DeleteUser = () => {
    const [email, setEmail] = useState('');
    const [status, setStatus] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const token = localStorage.getItem('jwtToken');
            const response = await axios.delete(`http://localhost:5001/api/UserService/DeleteUser?email=${email}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.status === 204) {
                setStatus('User deleted successfully.');
            } else {
                setStatus('User not found or error deleting.');
            }
        } catch (error) {
            setStatus('User not found or error deleting.');
        }
    };

    return (
        <div>
            <h2>Delete User</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Email:</label>
                    <input type="text" value={email} onChange={(e) => setEmail(e.target.value)} required />
                </div>
                <button type="submit">Delete User</button>
            </form>
            <p>Status: {status}</p>
            <Link to="/">Go to Main Page</Link>
        </div>
    );
}

export default DeleteUser;