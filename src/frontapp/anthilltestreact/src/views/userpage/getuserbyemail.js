import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';

const GetUserByEmail = () => {
    const [email, setEmail] = useState('');
    const [user, setUser] = useState(null);
    const [status, setStatus] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const token = localStorage.getItem('jwtToken');
            const response = await axios.get(`http://localhost:5001/api/UserService/GetUserByEmail?email=${email}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.status === 200) {
                setUser(response.data);
                setStatus(`User found: ${response.data.email}`);
            } else {
                setUser(null);
                setStatus('User not found.');
            }
        } catch (error) {
            setUser(null);
            setStatus('User not found.');
        }
    };

    return (
        <div>
            <h2>Get User by Email</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Email:</label>
                    <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
                </div>
                <button type="submit">Find User</button>
            </form>
            {user && (
                <div>
                    <h3>User Details:</h3>
                    <p>NickName: {user.nickName}</p>
                    <p>Email: {user.email}</p>
                    <p>Comments: {user.comments}</p>
                    <p>Create Date: {user.createDate}</p>
                </div>
            )}
            <p>Status: {status}</p>
            <Link to="/">Go to Main Page</Link>
        </div>
    );
}

export default GetUserByEmail;