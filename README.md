# TrueLayer-Pokemon

# How to run TrueLayer-Pokemon

Run the application local docker container 
1.) Download the Docker Desktop (https://hub.docker.com/editions/community/docker-ce-desktop-windows)

2.) Clone the source code from my git repository.

3.) Make sure your docker demon run your local.

4.) Run the following docker command to create docker image.
      docker build -t <dockerhub-account>/truelayer:1.0 -f src/TrueLayer.DataProduct.Pokemon.RestService/Dockerfile .
      
5.) Run the following docker comman run the application
      docker container run -d  -p 5000:5000 <dockerhub-account>/truelayer:1.0
      
 # For the production 
 1.) Authentication and Authorization In TrueLayer-Pokemon rest-api using  JSON Web Tokens.
 
 2.) Use Amazon API Gateway(Or any other Api gateway) to manage the rest full end-pint. 
 
 3.) Use AWS CloudWatch or ELK for the manage logs.
 
 4) For production deployment use AWS EKS or AWS ROSA. 
 
 
 
