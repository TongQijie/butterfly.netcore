# RabbitMQ Installation

安装RabbitMQ

``` bash
echo 'deb http://www.rabbitmq.com/debian/ stable main' | sudo tee /etc/apt/sources.list.d/rabbitmq.list

wget -O- https://www.rabbitmq.com/rabbitmq-release-signing-key.asc | sudo apt-key add -

sudo apt-get update

sudo apt-get install rabbitmq-server
```

启动RabbitMQ服务

``` bash
sudo systemctl status rabbitmq-server.service

sudo systemctl start rabbitmq-server.service

sudo systemctl stop rabbitmq-server.service
```

打开防火墙端口（如果需要）

``` bash
sudo ufw allow 5672/tcp
```

添加远程访问用户

``` bash
sudo rabbitmqctl add_user dev dev

sudo rabbitmqctl set_user_tags dev administrator

sudo rabbitmqctl set_permissions -p / dev '.*' '.*' '.*'
```