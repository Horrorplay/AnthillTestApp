import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';

const GetUserByNickName = () => {
    const [nickName, setNickName] = useState('');
    const [user, setUser] = useState(null);
    const [status, setStatus] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const token = localStorage.getItem('jwtToken');
            const response = await axios.get(`http://localhost:5001/api/UserService/GetUserByNickName?nickName=${nickName}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.status === 200) {
                setUser(response.data);
                setStatus(`User found: ${response.data.nickName}`);
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
            <h2>Get User by NickName</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>NickName:</label>
                    <input type="text" value={nickName} onChange={(e) => setNickName(e.target.value)} required />
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

export default GetUserByNickName;