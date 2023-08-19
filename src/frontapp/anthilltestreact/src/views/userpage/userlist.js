import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';

const UserList = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const token = localStorage.getItem('jwtToken');
                const response = await axios.get('http://localhost:5001/api/UserService/UserList', {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });
                setUsers(response.data);
                setLoading(false);
            } catch (error) {
                console.error('Error fetching user list:', error);
                setLoading(false);
            }
        };

        fetchUsers();
    }, []);

    return (
        <div>
            <h2>User List</h2>
            {loading ? (
                <p>Loading...</p>
            ) : (
                <ul>
                    {users.map((user) => (
                        <li key={user.email}>
                            <strong>NickName:</strong> {user.nickName}<br />
                            <strong>Email:</strong> {user.email}<br />
                            <strong>Comments:</strong> {user.comments}<br />
                            <strong>Create Date:</strong> {new Date(user.createDate).toLocaleString()}
                            <hr />
                        </li>
                    ))}
                </ul>
            )}
            <Link to="/">Go to Main Page</Link>
        </div>
    );
}

export default UserList;